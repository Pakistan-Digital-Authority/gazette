namespace gop.Responses.AdminResponses;

/// <summary>
/// To list the details of the users
/// </summary>
public class AdminUserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public string Ministry { get; set; }
}