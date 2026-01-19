using System.ComponentModel.DataAnnotations.Schema;
using gop.Enums;

namespace gop.Data.Entities;

public class Token : BaseEntity
{
    public Token(string access, string refresh, DateTime createdAt, DateTime expiresAt, TokenTypeEnum tokenType = TokenTypeEnum.Auth)
    {
        Access = access;
        Refresh = refresh;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        TokenType = tokenType;
    }

    public Token() { } // For ORM - Empty Constructor
    
    /// <summary>
    /// User identification.
    /// </summary>
    public Guid UserId { get; private init; }

    /// <summary>
    /// Access token (AccessToken), used to access the system.
    /// </summary>
    public string Access { get; private init; }

    /// <summary>
    /// Refresh token (RefreshToken), used to generate a new token.
    /// </summary>
    public string? Refresh { get; private init; } = string.Empty;

    /// <summary>
    /// Token Type - Define the token weather its login token or other.
    /// </summary>
    [Column(TypeName = "nvarchar(100)")]
    public TokenTypeEnum TokenType { get; private init; } = TokenTypeEnum.Auth;

    /// <summary>
    /// Token creation date.
    /// </summary>
    public DateTime CreatedAt { get; private init; }

    /// <summary>
    /// Token expiration date.
    /// </summary>
    public DateTime ExpiresAt { get; private init; }

    /// <summary>
    /// Revocation (cancellation) date of the token.
    /// </summary>
    public DateTime? RevokedAt { get; private set; }

    public User User { get; private init; }

    /// <summary>
    /// Indicates if the token has been revoked (cancelled).
    /// </summary>
    public bool IsRevoked =>
        RevokedAt.HasValue;

    /// <summary>
    /// Indicates whether the token is expired or revoked.
    /// </summary>
    /// <returns>Will return true if the token is valid otherwise it will return false if the token is revoked or expired.</returns>
    public bool IsValid() =>
        ExpiresAt >= DateTime.UtcNow && !IsRevoked;

    /// <summary>
    /// Revoke (cancel) the token.
    /// </summary>
    /// <param name="revocationDate">Revoke date.</param>
    public void Revoke(DateTime revocationDate)
    {
        if (!IsRevoked)
            RevokedAt = revocationDate;
    }
}