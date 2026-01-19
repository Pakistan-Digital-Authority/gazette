using System.ComponentModel.DataAnnotations;

namespace gop.Enums;

/// <summary>
/// To sort the notices
/// </summary>
public enum SortByEnum
{
    /// <summary>
    /// Will be sorted by latest
    /// </summary>
    [Display(Name = "Latest")]
    Latest,
    /// <summary>
    /// Will be sorted by oldest
    /// </summary>
    [Display(Name = "Oldest")]
    Oldest,
    /// <summary>
    /// Will be sorted by title
    /// </summary>
    [Display(Name = "Title")]
    Title,
    /// <summary>
    /// Will be sorted by ministry
    /// </summary>
    [Display(Name = "Ministry")]
    Ministry,
}