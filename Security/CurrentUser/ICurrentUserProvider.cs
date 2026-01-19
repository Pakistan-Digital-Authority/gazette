namespace gop.Security.CurrentUser;

/// <summary>
/// Current User Interface
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// The Current user
    /// </summary>
    /// <returns></returns>
    CurrentUser GetCurrentUser();

    /// <summary>
    /// Get Access Token Interface
    /// </summary>
    /// <returns></returns>
    string GetAccessToken();

    /// <summary>
    /// Get User Ip Address
    /// </summary>
    /// <returns></returns>
    string? GetIpAddress();
}