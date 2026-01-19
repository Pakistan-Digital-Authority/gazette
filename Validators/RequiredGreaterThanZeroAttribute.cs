using System.ComponentModel.DataAnnotations;

namespace gop.Validators;

/// <summary>
/// Greater than zero
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class RequiredGreaterThanZeroAttribute : ValidationAttribute
{
    /// <summary>
    /// Checking for validity
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool IsValid(object value) =>
        value != null && int.TryParse(value.ToString(), out var result) && result > 0;
}