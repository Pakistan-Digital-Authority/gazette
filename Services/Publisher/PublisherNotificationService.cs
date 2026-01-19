using gop.Data.Entities;
using gop.Enums;
using gop.Extensions;
using gop.Repositories;
using gop.Requests;
using gop.Responses;
using gop.Security.CurrentUser;
using gop.Utilities;

namespace gop.Services.Publisher;

/// <summary>
/// Publisher notification service interface
/// </summary>
public interface IPublisherNotificationService
{
    /// <summary>
    /// Get all notifications for current publisher
    /// </summary>
    Task<ApiResponse<PagedResult<PublisherNotificationResponse>>> GetAllAsync(GetPublisherNotificationsRequest request);

    /// <summary>
    /// Create a notification
    /// </summary>
    Task<ApiResponse> CreateAsync(CreateNotificationRequest request);
    
    /// <summary>
    /// Mark a notification as read
    /// </summary>
    Task<ApiResponse> MarkAsReadAsync(Guid id);

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    Task<ApiResponse> MarkAllAsReadAsync();
}

/// <summary>
/// Publisher notification service
/// </summary>
public class PublisherNotificationService : IPublisherNotificationService
{
    private readonly ICurrentUserProvider _currentUser;
    private readonly INotificationRepo _repo;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="currentUser"></param>
    /// <param name="repo"></param>
    public PublisherNotificationService(ICurrentUserProvider currentUser, INotificationRepo repo)
    {
        _currentUser = currentUser;
        _repo = repo;
    }

    /// <summary>
    /// Get all notifications
    /// </summary>
    public async Task<ApiResponse<PagedResult<PublisherNotificationResponse>>> GetAllAsync(GetPublisherNotificationsRequest request)
    {
        var response = new ApiResponse<PagedResult<PublisherNotificationResponse>>();
        try
        {
            var user = _currentUser.GetCurrentUser();

            var notifications = await _repo.GetPagedAsync(request.PageNumber, request.PageSize, user.Id);

            if (!notifications.Items.Any())
            {
                response.Status = (int)StatusCodeEnum.NotFound;
                response.Message = "No notifications found.";
                return response;
            }

            var result = notifications.Items.Select(n => new PublisherNotificationResponse
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                InitiatedTime = n.CreatedAt.ToRelativeTime()
            }).ToList();

            response.Status = 200;
            response.Message = "Notifications fetched successfully!";
            response.Data = new PagedResult<PublisherNotificationResponse>(
                result,
                notifications.TotalCount,
                notifications.PageNumber,
                notifications.PageSize);

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
    /// Create notification
    /// </summary>
    public async Task<ApiResponse> CreateAsync(CreateNotificationRequest request)
    {
        var response = new ApiResponse();

        try
        {
            var notification = new Notification
            {
                Title = request.Title,
                Message = request.Message,
                UserId = request.UserId,
                NoticeId = request.NoticeId ?? Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _repo.AddAsync(notification);

            if (!created)
            {
                response.Status = 500;
                response.Message = "Failed to create notification.";
                return response;
            }

            response.Status = 201;
            response.Message = "Notification created successfully.";
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
    /// Mark single notification as read
    /// </summary>
    public async Task<ApiResponse> MarkAsReadAsync(Guid id)
    {
        var response = new ApiResponse();

        try
        {
            var user = _currentUser.GetCurrentUser();
            var notification = await _repo.GetByIdAsync(id);

            if (notification == null || notification.UserId != user.Id)
            {
                response.Status = 404;
                response.Message = "Notification not found.";
                return response;
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _repo.UpdateAsync(notification);
            }

            response.Status = 200;
            response.Message = "Notification marked as read.";
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
    /// Mark all notifications as read
    /// </summary>
    public async Task<ApiResponse> MarkAllAsReadAsync()
    {
        var response = new ApiResponse();

        try
        {
            var user = _currentUser.GetCurrentUser();
            await _repo.MarkAllAsReadAsync(user.Id);

            response.Status = 200;
            response.Message = "All notifications marked as read.";
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