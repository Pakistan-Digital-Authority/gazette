using gop.Contracts;

namespace gop.Requests.AdminRequests;

/// <summary>
/// Audit And Logs Payload Request
/// </summary>
public class AdminAuditAndLogRequest : PagedRequest
{
    /// <summary>
    /// For search if any
    /// </summary>
    public string? Search { get; set; }
}