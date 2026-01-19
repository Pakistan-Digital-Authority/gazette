using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using gop.Data.Entities;
using gop.Enums;
using gop.Interfaces;

namespace gop.Security;

public class TokenGenerator
{
    private readonly ITokenClaimsService _tokenClaimsService;

    public TokenGenerator(ITokenClaimsService tokenClaimsService)
    {
        _tokenClaimsService = tokenClaimsService;
    }

    /// <summary>
    /// Generate Claims
    /// </summary>
    /// <param name="user"></param>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public Claim[] GenerateClaims(User user, string sessionId) =>
    [
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, user.FullName, ClaimValueTypes.String),
        new Claim(JwtRegisteredClaimNames.Email, user.Email, ClaimValueTypes.String),
        new Claim("Id", user.Id.ToString()),
        new Claim("FullName", user.FullName, ClaimValueTypes.String),
        new Claim("Email", user.Email, ClaimValueTypes.String),
        new Claim("Role", user.Role.ToString()),
        new Claim("SessionId", sessionId)
    ];

    private static Claim[] GenerateVerificationClaims(User user) =>
    [
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email.ToString(), ClaimValueTypes.Email)
    ];

    /// <summary>
    /// Generate Verification Token
    /// </summary>
    /// <param name="user"></param>
    /// <param name="tokenType"></param>
    /// <returns></returns>
    public Token GenerateVerificationToken(User user, TokenTypeEnum tokenType)
    {
        // Generating the rules (roles).
        var claims = GenerateVerificationClaims(user);
        // Generating the access token.
        var (accessToken, createdAt, expiresAt) = _tokenClaimsService.GenerateAccessToken(claims, 5000);
        // Linking the refresh token to the user.
        var newToken = new Token(accessToken, String.Empty, createdAt, expiresAt, tokenType);
        user.AddToken(newToken);
        return newToken;
    }
}