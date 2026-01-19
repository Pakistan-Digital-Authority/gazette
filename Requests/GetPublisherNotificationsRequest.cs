using gop.Contracts;

namespace gop.Requests;

/// <summary>
/// For notifications list
/// </summary>
public class GetPublisherNotificationsRequest : PagedRequest
{
    public string? Query { get; set; }
}