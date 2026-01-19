namespace gop.Responses.AdminResponses;

/// <summary>
/// Logs & Audit Response - List
/// </summary>
public class AuditAndLogListResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Status { get; set; }
    public string Title { get; set; }
    public string? Message { get; set; }
}