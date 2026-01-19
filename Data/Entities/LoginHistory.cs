namespace gop.Data.Entities;

public class LoginHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string? Message { get; set; }
    public string IpAddress { get; set; }
    public string DeviceInfo { get; set; }
    public DateTime LoginTime { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string SessionId { get; set; }
}