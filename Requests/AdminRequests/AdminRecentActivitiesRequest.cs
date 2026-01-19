using gop.Contracts;

namespace gop.Requests.AdminRequests;

/// <summary>
/// For recent activities
/// </summary>
public class AdminRecentActivitiesRequest : PagedRequest
{
    public string? Query { get; set; }
}