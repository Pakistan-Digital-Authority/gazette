using gop.Enums;

namespace gop.Responses.AuthResponses;

public class ProfileInfo
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public UserStatusEnum Status { get; set; }
}