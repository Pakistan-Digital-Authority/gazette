using gop.Contracts;

namespace gop.Requests.NoticeRequests;

/// <summary>
/// Get Publisher Notices List
/// </summary>
public class GetPublisherNoticesListRequest : PagedRequest
{
    public string? Query { get; set; }
}