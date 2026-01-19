using System.ComponentModel.DataAnnotations;

namespace gop.Enums;

/// <summary>
/// For Logs Level - i.e. Info, error etc
/// </summary>
public enum LogLevelEnum
{
    /// <summary>
    /// For Information type
    /// </summary>
    [Display(Name = "Info")]
    Info,
    /// <summary>
    /// For warning types
    /// </summary>
    [Display(Name = "Warning")]
    Warning,
    /// <summary>
    /// For errors
    /// </summary>
    [Display(Name =  "Error")]
    Error,
    /// <summary>
    /// Anything that is critical
    /// </summary>
    [Display(Name = "Critical")]
    Critical
}