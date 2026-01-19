namespace gop.Data.Entities;

/// <summary>
/// Notification model
/// </summary>
public class Notification
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid NoticeId { get; set; }
    public Notice Notice { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}