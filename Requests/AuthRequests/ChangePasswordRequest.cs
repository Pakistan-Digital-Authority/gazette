using System.ComponentModel.DataAnnotations;

namespace gop.Requests.AuthRequests;

/// <summary>
/// To change password - payload
/// </summary>
public class ChangePasswordRequest
{
    [Required]
    public string Email { get; set; }
}