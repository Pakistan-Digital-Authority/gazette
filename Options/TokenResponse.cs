namespace gop.Options;

public sealed class TokenResponse(Guid id, string accessToken, DateTime created, DateTime expiration, string refreshToken)
{
    /// <summary>
    /// Access token.
    /// </summary>
    public Guid Id { get; } = id;

    /// <summary>
    /// Access token.
    /// </summary>
    public string AccessToken { get; } = accessToken;

    /// <summary>
    /// Token creation date.
    /// </summary>
    public DateTime Created { get; } = created;

    /// <summary>
    /// Token expiration date.
    /// </summary>
    public DateTime Expiration { get; } = expiration;

    /// <summary>
    /// Refresh token.
    /// </summary>
    public string RefreshToken { get; } = refreshToken;

    /// <summary>
    /// Token expiration in seconds.
    /// </summary>
    public int ExpiresIn => (int)Expiration.Subtract(Created).TotalSeconds;
}