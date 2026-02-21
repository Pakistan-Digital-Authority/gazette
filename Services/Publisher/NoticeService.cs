using gop.Data.Entities;
using gop.Enums;
using gop.Extensions;
using gop.Interfaces;
using gop.Repositories;
using gop.Repositories.GeneralRepos;
using gop.Requests;
using gop.Requests.NoticeRequests;
using gop.Requests.PublicRequests;
using gop.Responses;
using gop.Responses.NoticeResponses;
using gop.Security.CurrentUser;
using gop.Utilities;

namespace gop.Services.Publisher;

/// <summary>
/// For notice service - interface
/// </summary>
public interface INoticeService
{
    /// <summary>
    /// Get all notices of the publisher
    /// </summary>
    /// <returns>Response with list of notices</returns>
    Task<ApiResponse<PagedResult<PublisherNoticeListResponse>>> GetAllAsync(GetPublisherNoticesListRequest request);
    
    /// <summary>
    /// Get all notices of the publisher - in drafts
    /// </summary>
    /// <returns>Response with list of draft notices</returns>
    Task<ApiResponse<PagedResult<PublisherNoticeListResponse>>> GetAllDraftsAsync(GetPublisherNoticesListRequest request);
    
    /// <summary>
    /// For search and filters
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<ApiResponse<PagedResult<PublicNoticesListResponse>>> GetSearchQueryResultAsync(PublicNoticesGetListRequest request);

    /// <summary>
    /// Get notice by ID
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <returns>Response with notice details</returns>
    Task<ApiResponse<PublisherNoticeResponse>> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Get Publisher Analytics
    /// </summary>
    /// <returns>Response with publisher analytics</returns>
    Task<ApiResponse<PublisherDashboardResponse>> GetDashboardAnalyticsAsync();

    /// <summary>
    /// Create a new notice
    /// </summary>
    /// <param name="request">Notice creation request</param>
    /// <returns>Response with created notice info or status</returns>
    Task<ApiResponse> CreateAsync(PublisherCreateNoticeRequest request);

    /// <summary>
    /// Update an existing notice
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <param name="request">Notice update request</param>
    /// <returns>Response with update status</returns>
    Task<ApiResponse> UpdateAsync(Guid id, PublisherUpdateNoticeRequest request);

    /// <summary>
    /// Delete a notice by ID
    /// </summary>
    /// <param name="id">Notice ID</param>
    /// <returns>Response with delete status</returns>
    Task<ApiResponse> DeleteAsync(Guid id);
}

/// <summary>
/// For notices
/// </summary>
public class NoticeService : INoticeService
{
    private readonly ICurrentUserProvider _currentUser;
    private readonly INoticeRepo _repo;
    private readonly IConfiguration _configuration;
    private readonly IFileStorageService _fileStorage;
    private readonly ISroNumberService _sroNumbering;
    private readonly IHostEnvironment _environment;
    private readonly IActReferenceRepo _actRepo;
    private readonly ILogsRepo _logsRepo;
    private readonly IPublicNoticeRepo _pubRepo;
    private readonly IUserRepo _userRepo;
    private readonly IPublisherNotificationService _notificationService;

    /// <summary>
    /// CTOR for notice service
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="repo"></param>
    /// <param name="configuration"></param>
    /// <param name="fileStorage"></param>
    /// <param name="sroNumbering"></param>
    /// <param name="environment"></param>
    /// <param name="actRepo"></param>
    /// <param name="logsRepo"></param>
    /// <param name="notificationService"></param>
    /// <param name="pubRepo"></param>
    /// <param name="userRepo"></param>
    public NoticeService(ICurrentUserProvider currentUser, INoticeRepo repo, IConfiguration configuration, IFileStorageService fileStorage, ISroNumberService sroNumbering, IHostEnvironment environment, IActReferenceRepo actRepo, ILogsRepo logsRepo, IPublisherNotificationService notificationService, IPublicNoticeRepo pubRepo, IUserRepo userRepo)
    {
        _currentUser = currentUser;
        _repo = repo;
        _configuration = configuration;
        _fileStorage = fileStorage;
        _sroNumbering = sroNumbering;
        _environment = environment;
        _actRepo = actRepo;
        _logsRepo = logsRepo;
        _notificationService = notificationService;
        _pubRepo = pubRepo;
        _userRepo = userRepo;
    }

    /// <summary>
    /// Get all notices of current publisher
    /// </summary>
    public async Task<ApiResponse<PagedResult<PublisherNoticeListResponse>>> GetAllAsync(GetPublisherNoticesListRequest request)
    {
        var response = new ApiResponse<PagedResult<PublisherNoticeListResponse>>();
        try
        {
            var user = _currentUser.GetCurrentUser();

            var notices = await _repo.GetPagedNoticesAsync(request.PageNumber, request.PageSize, user.Id.ToString(), "published");

            if (!notices.Items.Any())
            {
                response.Status = (int)StatusCodeEnum.NotFound;
                response.Message = "No notices found on the server!";
                return response;
            }

            var result = notices.Items.Select(n => new PublisherNoticeListResponse
            {
                Id = n.Id,
                Title = n.Title,
                Status = n.Status.ToString(),
                SroNumber = n.SroNumber,
                PublishedDateTime = n.PublishedDateTime.ToRelativeTime()
            }).ToList();
            
            var pagedResponse = new PagedResult<PublisherNoticeListResponse>(result, notices.TotalCount, notices.PageNumber, notices.PageSize);

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
    /// All drafts - notices
    /// </summary>
    /// <returns></returns>
    public async Task<ApiResponse<PagedResult<PublisherNoticeListResponse>>> GetAllDraftsAsync(GetPublisherNoticesListRequest request)
    {
        var response = new ApiResponse<PagedResult<PublisherNoticeListResponse>>();
        try
        {
            var user = _currentUser.GetCurrentUser();

            var notices = await _repo.GetPagedNoticesAsync(request.PageNumber, request.PageSize, user.Id.ToString(), "draft");
            
            if (!notices.Items.Any())
            {
                response.Status = (int)StatusCodeEnum.NotFound;
                response.Message = "No draft notices found on the server!";
                return response;
            }
            
            var result = notices.Items.Select(n => new PublisherNoticeListResponse
            {
                Id = n.Id,
                Title = n.Title,
                Status = n.Status.ToString(),
                SroNumber = n.SroNumber,
                PublishedDateTime = n.PublishedDateTime.ToRelativeTime()
            }).ToList();

            var pagedResponse = new PagedResult<PublisherNoticeListResponse>(result, notices.TotalCount, notices.PageNumber, notices.PageSize);

            response.Status = 200;
            response.Message = "Draft notices fetched successfully!";
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
    /// To search and filter - notices
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ApiResponse<PagedResult<PublicNoticesListResponse>>> GetSearchQueryResultAsync(PublicNoticesGetListRequest request)
    {
        var response = new ApiResponse<PagedResult<PublicNoticesListResponse>>();
        try
        {
            var user = _currentUser.GetCurrentUser();
            var userInfo = await _userRepo.GetByIdAsync(user.Id);
            if (userInfo is not null)
            {
                request.Ministry = userInfo.Ministry;
            }
            var notices = await _pubRepo.GetPagedNoticesAsync(request);

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
                PublishedDate = n.PublishedDateTime,
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
            var user = _currentUser.GetCurrentUser();
            var notice = await _repo.GetByIdAsync(id);

            if (notice == null || notice.UserId != user.Id)
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
                Description = notice.Description,
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
                // noticeDetail.PdfUrl = fileResponse.Location;
                noticeDetail.PdfUrl = "https://pdfobject.com/pdf/sample.pdf";
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
    /// Get Dashboard Analytics - Publisher
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ApiResponse<PublisherDashboardResponse>> GetDashboardAnalyticsAsync()
    {
        var response = new ApiResponse<PublisherDashboardResponse>();

        try
        {
            var user = _currentUser.GetCurrentUser();

            var dashboardResponse = new PublisherDashboardResponse();

            var totalPublishedNotices = await _repo.GetTotalNoticeCountAsync(user.Id, "published");
            var totalDraftNotices = await _repo.GetTotalNoticeCountAsync(user.Id, "draft");

            dashboardResponse.TotalPublishedCount = totalPublishedNotices;
            dashboardResponse.TotalDraftCount = totalDraftNotices;

            response.Status = 200;
            response.Message = "Dashboard analytics fetched successfully!";
            response.Data = dashboardResponse;
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
    /// Create notice
    /// </summary>
    public async Task<ApiResponse> CreateAsync(PublisherCreateNoticeRequest request)
    {
        var response = new ApiResponse();
        try
        {
            var currentUser = _currentUser.GetCurrentUser();

            #region -- Creating Notice

            var notice = new Notice
            {
                Title = request.Title,
                Description = request.Description,
                GazettePart = request.GazettePart,
                Keywords = request.Keywords,
                TemplateType = request.TemplateType,
                Content = request.Content,
                HtmlContent = request.HtmlContent,
                EffectiveDate = request.EffectiveDate,
                Status = request.Status,
            };
            
            var sroNumber = await _sroNumbering.GenerateAsync(SroPartEnum.I, DateTime.UtcNow.Year);
            notice.SroNumber = sroNumber.Numbering;

            var baseUrl = _environment.IsDevelopment() ? _configuration["FrontendUrl:Local"] : _configuration["FrontendUrl:Prod"];
            notice.Year = sroNumber.Year;
            notice.Slug = sroNumber.Slug;

            var fullPath = $"{baseUrl}/publisher/notice/{sroNumber.Slug}";
            notice.PreviewUrl = fullPath;
            
            // uploading file
            var basePath = _configuration["FilePaths:Notices"]!;
            var fileName = Guid.NewGuid().ToString() + "_" + request.PdfContent;
            var fullFilePath = Path.Combine(basePath, fileName);
            dynamic uploadedFile = await _fileStorage.UploadFileAsync(request.PdfContent, fullFilePath);

            if (uploadedFile.Status == 200)
            {
                notice.PdfUrl = fullFilePath;
            }
            
            notice.PublishedDateTime = DateTime.UtcNow;
            notice.UserId = currentUser.Id;
            notice.CreatedAt = DateTime.UtcNow;
            notice.UpdatedAt = DateTime.UtcNow;
            notice.Keywords = request.Keywords;
            notice.Tags = request.Tags;
            
            var created = await _repo.AddAsync(notice);
            if (!created)
            {
                response.Status = 500;
                response.Message = "Failed to create notice.";
                return response;
            }
            
            if (request.RelatedActs != null && request.RelatedActs.Any())
            {
                foreach (var relatedAct in request.RelatedActs)
                {
                    // Check if ActReference with the title exists
                    var actRef = await _actRepo.FindByTitleAsync(relatedAct);
                    if (actRef == null)
                    {
                        // Create new ActReference if not found
                        actRef = new ActReference
                        {
                            Title = relatedAct,
                        };
                        await _actRepo.AddAsync(actRef);
                    }

                    // Create linking entity
                    var noticeActReference = new NoticeActReference
                    {
                        ActId = actRef.Id,
                        NoticeId = notice.Id
                    };
                    notice.NoticeActReferences.Add(noticeActReference);
                }
            }

            #endregion
            
            #region -- Creating Logs

            var noticeDetails = await _repo.GetByIdAsync(notice.Id);

            if (noticeDetails is not null)
            {
                var log = new Log
                {
                    Level = LogLevelEnum.Info,
                    Title = request.Status == NoticeStatusEnum.Draft ? "Notice Saved As Draft" : "Notice Published",
                    Message = notice.SroNumber + " by " + noticeDetails.User.Ministry,
                    Source = nameof(NoticeService),
                    Action = nameof(CreateAsync),
                    HttpMethod = "POST",
                    RequestPath = "create-notice",
                    StatusCode = 200,
                    UserId = currentUser.Id,
                    UserEmail = currentUser.Email,
                    IpAddress = _currentUser.GetIpAddress(),
                    CreatedAt = DateTime.UtcNow
                };

                var createLog = await _logsRepo.CreateLogAsync(log);
                
                var auditLogs = new Log
                {
                    Level = LogLevelEnum.Info,
                    Title = notice.Status == NoticeStatusEnum.Draft ? notice.Title : notice.SroNumber,
                    Message = "User: " + currentUser.Email + " ● IP: " + _currentUser.GetIpAddress(),
                    LogType = LogTypeEnum.Audit,
                    Source = nameof(NoticeService),
                    Action = notice.Status == NoticeStatusEnum.Draft ? "DRAFT_SAVED" : "NOTICE_PUBLISHED",
                    HttpMethod = "POST",
                    RequestPath = "create-notice",
                    StatusCode = 200,
                    UserId = currentUser.Id,
                    UserEmail = currentUser.Email,
                    IpAddress = _currentUser.GetIpAddress(),
                    CreatedAt = DateTime.UtcNow
                };

                var auditLogsResponse = await _logsRepo.CreateLogAsync(auditLogs);
            }

            #endregion

            #region -- Creating Notification

            if (request.Status == NoticeStatusEnum.Published)
            {
                var newNotification = new CreateNotificationRequest
                {
                    Title = "Successfully Published",
                    Message = "Your gazette has been successfully published.",
                    UserId = currentUser.Id,
                    NoticeId = notice.Id
                };
                var notResponse = _notificationService.CreateAsync(newNotification);
            }
            else if (request.Status == NoticeStatusEnum.Draft)
            {
                var newNotification = new CreateNotificationRequest
                {
                    Title = "Notice Saved as Draft",
                    Message = "Your gazette has been successfully saved as draft.",
                    UserId = currentUser.Id,
                    NoticeId = notice.Id
                };
                var notResponse = _notificationService.CreateAsync(newNotification);
            }

            #endregion
            
            response.Status = 201;
            response.Message = "Notice created successfully!";
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
    /// Update notice
    /// </summary>
    public async Task<ApiResponse> UpdateAsync(Guid id, PublisherUpdateNoticeRequest request)
    {
        var response = new ApiResponse();

        try
        {
            var user = _currentUser.GetCurrentUser();

            #region -- Updating Notice

            var notice = await _repo.GetByIdAsync(id);
            
            if (notice == null || notice.UserId != user.Id)
            {
                response.Status = 404;
                response.Message = "Notice not found.";
                return response;
            }

            var previousStatus = notice.Status.ToString();
            
            notice.Title = request.Title;
            notice.Description = request.Description;
            notice.GazettePart = request.GazettePart;
            notice.Keywords = request.Keywords;
            notice.TemplateType = request.TemplateType;
            notice.Content = request.Content;
            notice.EffectiveDate = request.EffectiveDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
            notice.HtmlContent = request.HtmlContent;
            notice.UpdatedAt = DateTime.UtcNow;
            
            // uploading file
            var basePath = _configuration["FilePaths:Notices"]!;
            var fileName = Guid.NewGuid().ToString() + "_" + request.PdfContent;
            var fullFilePath = Path.Combine(basePath, fileName);
            dynamic uploadedFile = await _fileStorage.UploadFileAsync(request.PdfContent, fullFilePath);

            if (uploadedFile.Status == 200)
            {
                notice.PdfUrl = fullFilePath;
            }
            
            notice.Status = NoticeStatusEnum.Draft;
            notice.UpdatedAt = DateTime.UtcNow;
            notice.Keywords = request.Keywords;
            notice.Tags = request.Tags;

            var updated = await _repo.UpdateAsync(notice);
            if (!updated)
            {
                response.Status = 500;
                response.Message = "Failed to update notice.";
                return response;
            }

            #endregion
            
            //TODO:: need the related acts handling here - for update scenario (fully handled)

            #region -- Adding Notification + Logs

            if (request.Status == NoticeStatusEnum.Published && previousStatus == nameof(NoticeStatusEnum.Draft))
            {
                var newNotification = new CreateNotificationRequest
                {
                    Title = "Successfully Published",
                    Message = "Your gazette has been successfully published.",
                    UserId = user.Id,
                    NoticeId = notice.Id
                };
                var notResponse = _notificationService.CreateAsync(newNotification);
                
                #region -- Creating Logs

                var log = new Log
                {
                    Level = LogLevelEnum.Info,
                    Title = "Notice Published",
                    Message = notice.SroNumber + " by " + notice.User.Ministry,
                    Source = nameof(NoticeService),
                    Action = nameof(CreateAsync),
                    HttpMethod = "POST",
                    RequestPath = "create-notice",
                    StatusCode = 200,
                    UserId = user.Id,
                    UserEmail = user.Email,
                    IpAddress = _currentUser.GetIpAddress(),
                    CreatedAt = DateTime.UtcNow
                };
                var createLog = await _logsRepo.CreateLogAsync(log);
                
                var auditLogs = new Log
                {
                    Level = LogLevelEnum.Info,
                    Title = notice.SroNumber,
                    Message = "User: " + user.Email + " ● IP: " + _currentUser.GetIpAddress(),
                    LogType = LogTypeEnum.Audit,
                    Source = nameof(NoticeService),
                    Action = "NOTICE_PUBLISHED",
                    HttpMethod = "POST",
                    RequestPath = "create-notice",
                    StatusCode = 200,
                    UserId = user.Id,
                    UserEmail = user.Email,
                    IpAddress = _currentUser.GetIpAddress(),
                    CreatedAt = DateTime.UtcNow
                };

                var auditLogsResponse = await _logsRepo.CreateLogAsync(auditLogs);

                #endregion
            }

            #endregion

            response.Status = 200;
            response.Message = "Notice updated successfully!";
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
    /// Delete notice (soft delete)
    /// </summary>
    public async Task<ApiResponse> DeleteAsync(Guid id)
    {
        var response = new ApiResponse();
        try
        {
            var user = _currentUser.GetCurrentUser();
            var notice = await _repo.GetByIdAsync(id);

            if (notice == null || notice.UserId != user.Id)
            {
                response.Status = 404;
                response.Message = "Notice not found.";
                return response;
            }

            var deleted = await _repo.DeleteAsync(notice);
            if (!deleted)
            {
                response.Status = 500;
                response.Message = "Failed to delete notice.";
                return response;
            }

            response.Status = 200;
            response.Message = "Notice deleted successfully!";
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