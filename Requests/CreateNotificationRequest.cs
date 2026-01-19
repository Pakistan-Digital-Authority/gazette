namespace gop.Requests;

public class CreateNotificationRequest
{
    public Guid UserId { get; set; }
    public Guid? NoticeId { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
}
