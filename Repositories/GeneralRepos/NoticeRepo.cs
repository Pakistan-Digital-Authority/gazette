using gop.Data;
using gop.Data.Entities;
using gop.Enums;
using gop.Extensions;
using gop.Utilities;
using Microsoft.EntityFrameworkCore;

namespace gop.Repositories.GeneralRepos;

/// <summary>
/// Repo for notices
/// </summary>
public interface INoticeRepo
{
    /// <summary>
    /// to get the paginated list of notices
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="userId"></param>
    /// <param name="search"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task<PagedResult<Notice>> GetPagedNoticesAsync(int pageNumber, int pageSize, string? userId = null, string? status = null, string? search = null);
    
    /// <summary>
    /// To add the notice
    /// </summary>
    /// <param name="notice"></param>
    /// <returns></returns>
    Task<bool> AddAsync(Notice notice);
    
    /// <summary>
    /// To update the notice details
    /// </summary>
    /// <param name="notice"></param>
    /// <returns></returns>
    Task<bool> UpdateAsync(Notice notice);
    
    /// <summary>
    /// To delete/remove the notice
    /// </summary>
    /// <param name="notice"></param>
    /// <returns></returns>
    Task<bool> DeleteAsync(Notice notice);

    /// <summary>
    /// Get Total Notices Count
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task<int> GetTotalNoticeCountAsync(Guid userId, string status);
    
    /// <summary>
    /// To get the details of the notice by id
    /// </summary>
    /// <param name="noticeId"></param>
    /// <returns></returns>
    Task<Notice?> GetByIdAsync(Guid noticeId);
}

/// <summary>
/// Service for notices
/// </summary>
public class NoticeRepo : INoticeRepo
{
    private readonly DatabaseContext _context;
    /// <summary>
    /// CTOR - for Notice repo
    /// </summary>
    /// <param name="context"></param>
    public NoticeRepo(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// To get paginated list of notices
    /// </summary>
    public async Task<PagedResult<Notice>> GetPagedNoticesAsync(int pageNumber, int pageSize, string? userId, string? status = null, string? search = null)
    {
        var query = _context.Notices.Include(q => q.User).AsQueryable();
        
        query = query.Where(n => !n.IsDeleted);

        if (!string.IsNullOrWhiteSpace(userId))
        {
            var currentUserId = Guid.Parse(userId);
            query = query.Where(q => q.UserId == currentUserId);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status == "draft")
            {
                query = query.Where(q => q.Status == NoticeStatusEnum.Draft);   
            }
            else if (status == "published")
            {
                query = query.Where(q => q.Status == NoticeStatusEnum.Published);
            }
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(n =>
                n.Title.Contains(search) ||
                n.Description.Contains(search));
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }
    
    /// <summary>
    /// To add a notice
    /// </summary>
    public async Task<bool> AddAsync(Notice notice)
    {
        _context.Notices.Add(notice);
        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0;
    }

    /// <summary>
    /// To update notice details
    /// </summary>
    public async Task<bool> UpdateAsync(Notice notice)
    {
        _context.Notices.Update(notice);
        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0;
    }

    /// <summary>
    /// To soft delete a notice
    /// </summary>
    public async Task<bool> DeleteAsync(Notice notice)
    {
        notice.IsDeleted = true;
        notice.DeletedAt = DateTime.UtcNow;

        _context.Notices.Update(notice);
        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0;
    }

    /// <summary>
    /// To get the total count of notices
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<int> GetTotalNoticeCountAsync(Guid userId, string status)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var query = _context.Notices.AsQueryable();
        query = query.Where(n => n.UserId == userId && !n.IsDeleted && n.CreatedAt >= thirtyDaysAgo);
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (status.Equals("published", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(n => n.Status == NoticeStatusEnum.Published);
            }
            else if (status.Equals("draft", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(n => n.Status == NoticeStatusEnum.Draft);
            }
            else
            {
                return 0;
            }
        }
        return await query.CountAsync();
    }

    /// <summary>
    /// To get notice details by id
    /// </summary>
    public async Task<Notice?> GetByIdAsync(Guid noticeId)
    {
        return await _context.Notices
            .Include(q => q.User)
            .Include(n => n.NoticeActReferences)
                .ThenInclude(ar => ar.ActReference)
            .Where(n => !n.IsDeleted)
            .FirstOrDefaultAsync(n => n.Id == noticeId);
    }
}