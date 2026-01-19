namespace gop.Responses;

/// <summary>
/// To list down publisher notifications
/// </summary>
public class PublisherNotificationResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public string InitiatedTime { get; set; }
}