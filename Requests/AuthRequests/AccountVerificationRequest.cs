namespace gop.Requests.AuthRequests;

/// <summary>
/// To verify the account
/// </summary>
public class AccountVerificationRequest
{
    /// <summary>
    /// Token for account verification
    /// </summary>
    public string Token { get; set; }
}