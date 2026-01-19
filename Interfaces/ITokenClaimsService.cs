using System.Security.Claims;
using gop.Records;

namespace gop.Interfaces;

/// <summary>
/// Token Claim Creation Service
/// </summary>
public interface ITokenClaimsService
{
    /// <summary>
    /// Generating Access Token
    /// </summary>
    /// <param name="claims"></param>
    /// <param name="expiresInSeconds"></param>
    /// <returns></returns>
    AccessToken GenerateAccessToken(Claim[] claims, int? expiresInSeconds = 21600);
    /// <summary>
    /// Generating Refresh Token
    /// </summary>
    /// <returns></returns>
    string GenerateRefreshToken();
}