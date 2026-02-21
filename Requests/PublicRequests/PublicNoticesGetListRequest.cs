using gop.Contracts;
using gop.Enums;

namespace gop.Requests.PublicRequests;

/// <summary>
/// To get all the available notices/gazettes
/// </summary>
public class PublicNoticesGetListRequest : PagedRequest
{
    public string? Query { get; set; }
    public string? Year { get; set; } // can be all year if null
    public string? NoticeType { get; set; }
    public string? GazettePart { get; set; }
    public string? Ministry { get; set; } // can be all ministry if null
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
    public SortByEnum SortBy { get; set; } = SortByEnum.Latest;
    public ICollection<string>? Tags { get; set; }
}