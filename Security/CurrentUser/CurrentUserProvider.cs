using System.Security.Claims;
using Throw;

namespace gop.Security.CurrentUser;

/// <summary>
/// For Current User Info
/// </summary>
/// <param name="httpContextAccessor"></param>
public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    /// <summary>
    /// The Current user Info extraction
    /// </summary>
    /// <returns></returns>
    public CurrentUser GetCurrentUser()
    {
        httpContextAccessor.HttpContext.ThrowIfNull();
        var id = Guid.Parse(GetSingleClaimValue(ClaimTypes.NameIdentifier));
        var email = GetSingleClaimValue(ClaimTypes.Email);
        var fullName = GetSingleClaimValue("FullName");
        var role = GetSingleClaimValue("Role");
        var sessionId = GetSingleClaimValue("SessionId");

        return new CurrentUser(id, fullName, email, role, sessionId);
    }
    
    /// <summary>
    /// Return the bearer token
    /// </summary>
    /// <returns></returns>
    public string GetAccessToken()
    {
        var httpContext = httpContextAccessor.HttpContext;
        httpContext.ThrowIfNull();

        var header = httpContext.Request.Headers["Authorization"].ToString();
        return header["Bearer ".Length..].Trim();
    }

    /// <summary>
    /// Get User IP Address
    /// </summary>
    /// <returns></returns>
    public string? GetIpAddress()
    {
        var ipAddress = httpContextAccessor.HttpContext?.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        return ipAddress;
    }

    private List<string> GetClaimValues(string claimType) =>
        httpContextAccessor.HttpContext!.User.Claims
            .Where(claim => claim.Type == claimType)
            .Select(claim => claim.Value)
            .ToList();

    private string GetSingleClaimValue(string claimType)
    {
        var claimValue = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(c => c.Type == claimType)!.Value;
        return claimValue;
    }
}