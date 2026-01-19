using System.ComponentModel.DataAnnotations;

namespace gop.Requests.AuthRequests;

public class AcceptInvitationRequest
{
    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; }
    
    [Required(ErrorMessage = "New password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$", ErrorMessage = "Password must contain uppercase, lowercase, number, and special character.")]
    public string NewPassword { get; set; }
    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("NewPassword", ErrorMessage = "New password and confirm password must match.")]
    public string ConfirmPassword { get; set; }
}