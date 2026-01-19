using gop.Enums;

namespace gop.Requests.AdminRequests;

/// <summary>
/// A payload for user creation - through admin
/// </summary>
public class AdminCreateUserRequest
{
    public string Name { get; set; }
    public string Ministry { get; set; }
    public string Email { get; set; }
    public UserStatusEnum Status { get; set; }
    public UserRoleEnum Role { get; set; } =  UserRoleEnum.Publisher;
}