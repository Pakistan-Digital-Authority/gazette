namespace gop.Services;

/// <summary>
/// DateTime Provider
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// UTC Now
    /// </summary>
    DateTime UtcNow { get; }
    /// <summary>
    /// Now
    /// </summary>
    DateTime Now { get; }
}

/// <summary>
/// DateTime Provider Implementation
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// UTC Now
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
    /// <summary>
    /// Now
    /// </summary>
    public DateTime Now => DateTime.Now;
}