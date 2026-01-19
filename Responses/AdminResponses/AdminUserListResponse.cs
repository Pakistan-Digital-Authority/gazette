namespace gop.Responses.AdminResponses;

/// <summary>
/// To list down all the users
/// </summary>
public class AdminUserListResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Ministry { get; set; }
    public string Status { get; set; }
    public string? Phone { get; set; }
}