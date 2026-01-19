using System.ComponentModel.DataAnnotations;

namespace gop.Enums;
/// <summary>
/// Token Types - While creating JWT
/// </summary>
public enum TokenTypeEnum
{
    /// <summary>
    /// Token used for authentication only
    /// </summary>
    [Display(Name = "Auth")]
    Auth,
    /// <summary>
    /// This type of token will be used only to reset passwords
    /// </summary>
    [Display(Name = "ResetPassword")]
    ResetPassword,
    /// <summary>
    /// The token with this type will be only used for account verification
    /// </summary>
    [Display(Name = "AccountVerification")]
    AccountVerification,
    /// <summary>
    /// For account invitation - added by admin
    /// </summary>
    [Display(Name = "Invitation")]
    Invitation
}