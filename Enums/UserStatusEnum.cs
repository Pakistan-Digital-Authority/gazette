using System.ComponentModel.DataAnnotations;

namespace gop.Enums;

/// <summary>
/// For User Status
/// </summary>
public enum UserStatusEnum
{
    /// <summary>
    /// The user account is created but not yet activated
    /// </summary>
    [Display(Name = "Pending")]
    Pending,
    /// <summary>
    /// The user account is active and allowed to log in.
    /// </summary>
    [Display(Name = "Active")]
    Active,
    /// <summary>
    /// The user account has been blocked due to policy or violations.
    /// </summary>
    [Display(Name = "Blocked")]
    Blocked,
    /// <summary>
    /// The user account has been temporarily suspended.
    /// </summary>
    [Display(Name = "Suspended")]
    Suspended,
    /// <summary>
    /// User account is locked
    /// </summary>
    [Display(Name = "Locked")]
    Locked,
}