using gop.Data;
using gop.Data.Entities;
using gop.Enums;
using gop.Extensions;
using gop.Utilities;
using Microsoft.EntityFrameworkCore;

namespace gop.Repositories.GeneralRepos;

/// <summary>
/// Logs Repo
/// </summary>
public interface ILogsRepo
{
    /// <summary>
    /// Get paginated list of logs
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="type"></param>
    /// <param name="search"></param>
    /// <returns></returns>
    Task<PagedResult<Log>> GetPagedLogsAsync(int pageNumber, int pageSize, string? type = null, string? search = null);

    /// <summary>
    /// Create Log
    /// </summary>
    /// <param name="log"></param>
    /// <returns></returns>
    Task<bool> CreateLogAsync(Log log);
}

/// <summary>
/// Logs Repo - implementation
/// </summary>
public class LogsRepo : ILogsRepo
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// CTOR - Logs repo
    /// </summary>
    public LogsRepo(DatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get paginated logs
    /// </summary>
    public async Task<PagedResult<Log>> GetPagedLogsAsync(int pageNumber, int pageSize, string? type = null, string? search = null)
    {
        var query = _context.Logs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(l =>
                l.Source != null && (l.Title.Contains(search) ||
                                     l.Source.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            if (type == "audit")
            {
                query = query.Where(q => q.LogType == LogTypeEnum.Audit);
            }
        }
        else
        {
            query = query.Where(q => q.LogType == LogTypeEnum.General);
        }

        return await query
            .OrderByDescending(l => l.CreatedAt)
            .AsNoTracking()
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <summary>
    /// Create log
    /// </summary>
    /// <param name="log"></param>
    public async Task<bool> CreateLogAsync(Log log)
    {
        await _context.Logs.AddAsync(log);
        var affectedRows = await _context.SaveChangesAsync();
        return affectedRows > 0;
    }
}