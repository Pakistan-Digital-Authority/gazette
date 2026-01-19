using gop.Enums;
using gop.Extensions;
using gop.Interfaces;
using gop.Repositories;
using gop.Repositories.GeneralRepos;
using gop.Requests.NoticeRequests;
using gop.Requests.PublicRequests;
using gop.Responses;
using gop.Responses.NoticeResponses;
using gop.Security.CurrentUser;
using gop.Services.Publisher;
using gop.Utilities;

namespace gop.Services.Public;

/// <summary>
/// For public api's
/// </summary>
public interface IPublicNoticeService
{
    /// <summary>
    /// Get all notices of the publisher
    /// </summary>
    /// <returns>Response with list of notices</returns>
    Task<ApiResponse<PagedResult<PublicNoticesListResponse>>> GetAllAsync(PublicNoticesGetListRequest request);
    
    /// <summary>
    /// Get notice by ID
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <returns>Response with notice details</returns>
    Task<ApiResponse<PublisherNoticeResponse>> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Get notice pdf by Notice ID
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <returns>Response with notice pdf file url</returns>
    Task<ApiResponse<PublicNoticePdfUrlResponse>> GetNoticePdfFiledAsync(Guid id);
}

/// <summary>
/// Public Notice Service
/// </summary>
public class PublicNoticeService : IPublicNoticeService
{
    private readonly IPublicNoticeRepo _repo;
    private readonly IFileStorageService _fileStorage;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="fileStorage"></param>
    public PublicNoticeService(IPublicNoticeRepo repo, IFileStorageService fileStorage)
    {
        _repo = repo;
        _fileStorage = fileStorage;
    }
    
    /// <summary>
    /// Get all notices of current publisher
    /// </summary>
    public async Task<ApiResponse<PagedResult<PublicNoticesListResponse>>> GetAllAsync(PublicNoticesGetListRequest request)
    {
        var response = new ApiResponse<PagedResult<PublicNoticesListResponse>>();
        try
        {
            var notices = await _repo.GetPagedNoticesAsync(request);

            if (!notices.Items.Any())
            {
                response.Status = (int)StatusCodeEnum.NotFound;
                response.Message = "No notices found on the server!";
                return response;
            }

            var result = notices.Items.Select(n => new PublicNoticesListResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                GazettePart = n.GazettePart,
                Ministry = n.User.Ministry,
                SroNumber = n.SroNumber,
                PubslishedDate = n.PublishedDateTime,
                Tags = n.Tags,
                PublishedDateTime = n.PublishedDateTime.ToRelativeTime()
            }).ToList();
            
            var pagedResponse = new PagedResult<PublicNoticesListResponse>(result, notices.TotalCount, notices.PageNumber, notices.PageSize);

            response.Status = 200;
            response.Message = "Published notices fetched successfully!";
            response.Data = pagedResponse;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }
    
    /// <summary>
    /// Get notice by id
    /// </summary>
    public async Task<ApiResponse<PublisherNoticeResponse>> GetByIdAsync(Guid id)
    {
        var response = new ApiResponse<PublisherNoticeResponse>();

        try
        {
            var notice = await _repo.GetByIdAsync(id);

            if (notice == null)
            {
                response.Status = 404;
                response.Message = "Notice not found.";
                return response;
            }
            
            var noticeDetail = new PublisherNoticeResponse
            {
                Id = notice.Id,
                Title = notice.Title,
                HtmlContent = notice.HtmlContent,
                Status = notice.Status,
                PublishedDateTime = notice.PublishedDateTime,
                EffectiveDate = notice.EffectiveDate,
                SroNumber = notice.SroNumber,
                Ministry = notice.User.Ministry,
                GazettePart = notice.GazettePart,
                IssuingAuthorityName = notice.User.FullName,
                PreviewUrl = notice.PreviewUrl,
                Tags = notice.Tags
            };
            noticeDetail.ActReference = notice.NoticeActReferences.Select(ar => new RelatedActListResponse
            {
                Id = ar.ActReference.Id,
                Title = ar.ActReference.Title
            }).ToList();

            dynamic fileResponse = await _fileStorage.GetFileUrlAsync(notice.PdfUrl);
            if (fileResponse.Status == 200)
            {
                noticeDetail.PdfUrl = fileResponse.Location;
            }

            response.Status = 200;
            response.Message = "Notice details fetched successfully!";
            response.Data = noticeDetail;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }

    /// <summary>
    /// To get the notice file url
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ApiResponse<PublicNoticePdfUrlResponse>> GetNoticePdfFiledAsync(Guid id)
    {
        var response = new ApiResponse<PublicNoticePdfUrlResponse>();
        try
        {
            var notice = await _repo.GetByIdAsync(id);
            if (notice == null)
            {
                response.Status = 404;
                response.Message = "Notice not found.";
                return response;
            }

            var pdfFileResponse = new PublicNoticePdfUrlResponse();
            dynamic fileResponse = await _fileStorage.GetFileUrlAsync(notice.PdfUrl);
            if (fileResponse.Status == 200)
            {
                pdfFileResponse.FileUrl = fileResponse.Location;
            }

            response.Status = 200;
            response.Message = "Notice file url fetched successfully!";
            response.Data = pdfFileResponse;
            return response;
        }
        catch (Exception ex)
        {
            response.Status = 500;
            response.Message = ex.Message;
            return response;
        }
    }
}