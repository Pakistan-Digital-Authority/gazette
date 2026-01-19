using System.ComponentModel.DataAnnotations;

namespace gop.Enums;

/// <summary>
/// Status for the notices
/// </summary>
public enum NoticeStatusEnum
{
    /// <summary>
    /// For an active/published notice
    /// </summary>
    [Display(Name = "Published")]
    Published,
    /// <summary>
    /// For a notice in draft mode
    /// </summary>
    [Display(Name = "Draft")]
    Draft,
}