namespace gop.Security.CurrentUser;

public record CurrentUser(
    Guid Id,
    string FullName,
    string Email,
    string Role,
    string SessionId);