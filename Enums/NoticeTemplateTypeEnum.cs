using System.ComponentModel.DataAnnotations;

namespace gop.Enums;

/// <summary>
/// Gazette Template Type 
/// </summary>
public enum NoticeTemplateTypeEnum
{
    /// <summary>
    /// For a gazette with blank template
    /// </summary>
    [Display(Name = "Blank")]
    Blank,
    /// <summary>
    /// For a gazette with act/sro template type
    /// </summary>
    [Display(Name = "ACT/SRO")]
    ActSro
}