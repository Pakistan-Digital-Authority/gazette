using gop.Data;
using gop.Data.Entities;
using gop.Extensions;
using gop.Utilities;
using Microsoft.EntityFrameworkCore;

namespace gop.Repositories;

/// <summary>
/// interface
/// </summary>
public interface INotificationRepo
{
    /// <summary>
    /// To get all the notifications - paginated
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<PagedResult<Notification>> GetPagedAsync(int pageNumber, int pageSize, Guid userId);

    /// <summary>
    /// To get notification by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Notification?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Add notification
    /// </summary>
    Task<bool> AddAsync(Notification notification);
    
    /// <summary>
    /// To update - mark all as read
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(Notification entity);
    
    /// <summary>
    /// To mark all notifications as read
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task MarkAllAsReadAsync(Guid userId);
}

/// <summary>
/// Service
/// </summary>
public class NotificationRepo : INotificationRepo
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="context"></param>
    public NotificationRepo(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// To list down all the notification in paginated view
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<PagedResult<Notification>> GetPagedAsync(int pageNumber, int pageSize, Guid userId)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt);
        var totalCount = await query.CountAsync();

        var items = await query
            .AsNoTracking()
            .ToPagedResultAsync(pageNumber, pageSize);

        return items;
    }
    
    /// <summary>
    /// To get notification by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
    }
    
    /// <summary>
    /// Add notification
    /// </summary>
    public async Task<bool> AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        return await _context.SaveChangesAsync() > 0;
    }
    
    /// <summary>
    /// To mark all notificaitons a read
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(Notification entity)
    {
        _context.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// To mark all as read
    /// </summary>
    /// <param name="userId"></param>
    public async Task MarkAllAsReadAsync(Guid userId)
    {
        await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsRead, true));
    }
}