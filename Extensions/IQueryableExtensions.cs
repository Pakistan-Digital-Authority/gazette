using gop.Utilities;
using Microsoft.EntityFrameworkCore;

namespace gop.Extensions;

/// <summary>
/// Query to get the paginated data
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Get the paginated data
    /// </summary>
    /// <param name="query"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        int skipCount = pageNumber * pageSize;
        var items = await query
            .Skip(skipCount)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }
}