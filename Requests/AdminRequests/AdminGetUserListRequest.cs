using gop.Contracts;

namespace gop.Requests.AdminRequests;

/// <summary>
/// To get the user list
/// </summary>
public class AdminGetUserListRequest : PagedRequest
{
    public string? Query { get; set; }
}