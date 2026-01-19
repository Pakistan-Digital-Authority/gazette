namespace gop.Contracts;

/// <summary>
/// For pagination request
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// The number of the pages
    /// </summary>
    public int PageNumber { get; set; } = 0;
    /// <summary>
    /// Current Page size
    /// </summary>
    public int PageSize { get; set; } = 10;
}