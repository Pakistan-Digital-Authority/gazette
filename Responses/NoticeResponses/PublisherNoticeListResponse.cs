namespace gop.Responses.NoticeResponses;

/// <summary>
/// To list down the notices
/// </summary>
public class PublisherNoticeListResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string? SroNumber { get; set; }
    public string PublishedDateTime { get; set; }
}