namespace gop.Utilities;

/// <summary>
/// Paged Result - for paginated data
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }

    public PagedResult(List<T> items, int count, int pageNumber, int pageSize)
    {
        CurrentPage = pageNumber + 1;
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        HasNext = CurrentPage < TotalPages;
        HasPrevious = CurrentPage > 1;
    }
}