using gop.Enums;

namespace gop.Requests.AdminRequests;

public class AdminUpdateUserRequest
{
    public string Name { get; set; }
    // public string Email { get; set; }
    public string? Phone { get; set; }
    public string Ministry { get; set; }
    public UserStatusEnum Status { get; set; }
    public UserRoleEnum Role { get; set; }
}