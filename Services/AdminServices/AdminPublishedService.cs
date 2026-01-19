using gop.Extensions;
using gop.Repositories.GeneralRepos;
using gop.Requests.AdminRequests;
using gop.Responses.AdminResponses;
using gop.Security.CurrentUser;
using gop.Utilities;

namespace gop.Services.AdminServices;

/// <summary>
/// Interface for published notices
/// </summary>
public interface IAdminPublishedService
{
    /// <summary>
    /// Get Published notices list
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse<PagedResult<PublishedNoticesListResponse>>> GetPublishedNoticesAsync(PublishedNoticesListRequest request);
    
    /// <summary>
    /// Get recent activities list
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse<PagedResult<AdminRecentActivityResponse>>> GetRecentActivitiesAsync(AdminRecentActivitiesRequest request);
}

/// <summary>
/// Admin published notices service
/// </summary>
public class AdminPublishedService : IAdminPublishedService
{
    private readonly INoticeRepo _repo;
    private readonly ILogsRepo _logsRepo;
    private readonly ICurrentUserProvider _currentUser;

    /// <summary>
    /// CTR for admin publisher
    /// </summary>
    /// <param name="repo"></param>
    /// <param name="currentUser"></param>
    /// <param name="logsRepo"></param>
    public AdminPublishedService(INoticeRepo repo, ICurrentUserProvider currentUser, ILogsRepo logsRepo)
    {
        _repo = repo;
        _currentUser = currentUser;
        _logsRepo = logsRepo;
    }
    
    /// <summary>
    /// List of published notices - for admin
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse<PagedResult<PublishedNoticesListResponse>>> GetPublishedNoticesAsync(PublishedNoticesListRequest request)
    {
        var response = new ApiResponse<PagedResult<PublishedNoticesListResponse>>();
        try
        {
            var notices = await _repo.GetPagedNoticesAsync(request.PageNumber, request.PageSize, null, "published");
            
            if (!notices.Items.Any())
            {
                response.Status = 404;
                response.Message = "No notices found on the server!";
                return response;
            }

            var noticesList = new List<PublishedNoticesListResponse>();
            foreach (var notice in notices.Items)
            {
                var newNotice = new PublishedNoticesListResponse();
                newNotice.Id = notice.Id;
                newNotice.Title = notice.Title;
                newNotice.Ministry = notice.User.Ministry;
                newNotice.PublishedDate = notice.PublishedDateTime;
                newNotice.SroNumber = notice.SroNumber;
                newNotice.Status = notice.Status.ToString();
                
                noticesList.Add(newNotice);
            }
            
            var pagedResponse = new PagedResult<PublishedNoticesListResponse>(noticesList, notices.TotalCount, notices.PageNumber, notices.PageSize);
            
            response.Status = 200;
            response.Message = "Notices fetched successfully!";
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
    /// To get all the recent activities for admin
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<ApiResponse<PagedResult<AdminRecentActivityResponse>>> GetRecentActivitiesAsync(AdminRecentActivitiesRequest request)
    {
        var response = new ApiResponse<PagedResult<AdminRecentActivityResponse>>();
        try
        {
            var logs = await _logsRepo.GetPagedLogsAsync(request.PageNumber, request.PageSize);
            
            if (!logs.Items.Any())
            {
                response.Status = 404;
                response.Message = "No recent activities found on the server!";
                return response;
            }

            var logsList = new List<AdminRecentActivityResponse>();
            foreach (var log in logs.Items)
            {
                var newLog = new AdminRecentActivityResponse
                {
                    Title = log.Title,
                    Message = log.Message ?? "",
                    InitiatedTime = log.CreatedAt.ToRelativeTime()
                };

                logsList.Add(newLog);
            }
            
            var pagedResponse = new PagedResult<AdminRecentActivityResponse>(logsList, logs.TotalCount, logs.PageNumber, logs.PageSize);
            
            response.Status = 200;
            response.Message = "Recent activities fetched successfully!";
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
}