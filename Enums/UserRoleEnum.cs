using System.ComponentModel.DataAnnotations;

namespace gop.Enums;

/// <summary>
/// Enums that define role for the user
/// </summary>
public enum UserRoleEnum
{
    /// <summary>
    /// Role for admin
    /// </summary>
    [Display(Name = "Admin")]
    Admin,
    /// <summary>
    /// Role for publisher
    /// </summary>
    [Display(Name = "Publisher")]
    Publisher,
    /// <summary>
    /// For Approver
    /// </summary>
    [Display(Name = "Approver")]
    Approver,
}