using gop.Validators;

namespace gop.Options;

/// <summary>
/// Auth Options
/// </summary>
public sealed class AuthOptions : IAppOptions
{
    /// <summary>
    /// Config Section
    /// </summary>
    public static string ConfigSectionPath => "AuthOptions";

    /// <summary>
    /// Maximum attempts that a user can make
    /// </summary>
    [RequiredGreaterThanZero]
    public int MaximumAttempts { get; private init; }

    /// <summary>
    /// Seconds - blocked for
    /// </summary>
    [RequiredGreaterThanZero]
    public int SecondsBlocked { get; private init; }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="maximumAttempts"></param>
    /// <param name="secondsBlocked"></param>
    /// <returns></returns>
    public static AuthOptions Create(int maximumAttempts, int secondsBlocked)
        => new() { MaximumAttempts = maximumAttempts, SecondsBlocked = secondsBlocked };
}