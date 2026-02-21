using gop.Data;
using gop.Data.Entities;
using gop.Enums;
using gop.Extensions;
using gop.Requests.PublicRequests;
using gop.Utilities;
using Microsoft.EntityFrameworkCore;

namespace gop.Repositories;

/// <summary>
/// interface
/// </summary>
public interface IPublicNoticeRepo
{
    /// <summary>
    /// to get the paginated list of notices
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<PagedResult<Notice>> GetPagedNoticesAsync(PublicNoticesGetListRequest request);
    
    /// <summary>
    /// To get the details of the notice by id
    /// </summary>
    /// <param name="noticeId"></param>
    /// <returns></returns>
    Task<Notice?> GetByIdAsync(Guid noticeId);
}

/// <summary>
/// REPO
/// </summary>
public class PublicNoticeRepo : IPublicNoticeRepo
{
    private readonly DatabaseContext _context;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="context"></param>
    public PublicNoticeRepo(DatabaseContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// To get paginated list of notices
    /// </summary>
    public async Task<PagedResult<Notice>> GetPagedNoticesAsync(PublicNoticesGetListRequest request)
    {
        var query = _context.Notices.Include(q => q.User).AsQueryable();
        
        query = query.Where(n => !n.IsDeleted);
        
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            query = query.Where(q => q.Title.Contains(request.Query) || q.SroNumber.Contains(request.Query) || (q.Keywords != null && q.Keywords.Contains(request.Query)));
        }

        if (!string.IsNullOrWhiteSpace(request.Year))
        {
            query = query.Where(q => q.Year.ToString() == request.Year);
        }
        
        // need to discuss this
        // if (!string.IsNullOrWhiteSpace(request.NoticeType))
        // {
        //     query = query.Where(q => q.TemplateType.ToString() == request.NoticeType);
        // }

        if (!string.IsNullOrWhiteSpace(request.GazettePart))
        {
            var part = request.GazettePart.Trim();
            query = query.Where(q => q.GazettePart == part);
        }
        
        if (!string.IsNullOrWhiteSpace(request.Ministry))
        {
            query = query.Where(q => q.User.Ministry.Contains(request.Ministry));
        }
        
        if (request.DateFrom.HasValue &&  request.DateTo.HasValue)
        {
            DateTime fromDate = request.DateFrom.Value.ToDateTime(TimeOnly.MinValue);
            DateTime toDate = request.DateTo.Value.ToDateTime(TimeOnly.MaxValue);
            
            query = query.Where(q => 
                q.PublishedDateTime >= fromDate &&
                q.PublishedDateTime <= toDate
            );
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy.ToString()))
        {
            switch (request.SortBy)
            {
                case SortByEnum.Latest:
                    query = query.OrderByDescending(q => q.PublishedDateTime);
                    break;
                case SortByEnum.Oldest:
                    query = query.OrderBy(q => q.PublishedDateTime);
                    break;
                case SortByEnum.Title:
                    query = query.OrderBy(q => q.Title);
                    break;
                case SortByEnum.Ministry:
                    query = query.OrderBy(q => q.User.Ministry);
                    break;
                default:
                    query = query.OrderByDescending(q => q.PublishedDateTime);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(n => n.PublishedDateTime);
        }
        
        if (request.Tags?.Any() == true)
        {
            query = query.Where(q =>
                request.Tags.Any(tag => q.Tags.Contains(tag))
            );
        }

        return await query.ToPagedResultAsync(request.PageNumber, request.PageSize);
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
            .Where(n => !n.IsDeleted && n.Status == NoticeStatusEnum.Published)
            .FirstOrDefaultAsync(n => n.Id == noticeId);
    }
}