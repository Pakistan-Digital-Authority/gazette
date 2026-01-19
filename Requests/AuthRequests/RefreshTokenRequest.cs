namespace gop.Requests.AuthRequests;

/// <summary>
/// Refresh Token Request
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// The Token to Refresh it
    /// </summary>
    public string Token { get; set; }
}