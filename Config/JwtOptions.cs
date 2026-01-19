using System.ComponentModel.DataAnnotations;
using gop.Options;
using gop.Validators;

namespace gop.Config;

/// <summary>
/// JWT Options class
/// </summary>
public sealed class JwtOptions : IAppOptions
{
    /// <summary>
    /// The JWT Options
    /// </summary>
    public static string ConfigSectionPath => "JwtOptions";

    /// <summary>
    /// aud: Defines who can use the token.
    /// </summary>
    [Required]
    public string Audience { get; private init; }

    /// <summary>
    /// iss: The domain of the application generating the token.
    /// </summary>
    [Required]
    public string Issuer { get; private init; }

    /// <summary>
    /// Token lifespan in seconds.
    /// </summary>
    [RequiredGreaterThanZero]
    public int Seconds { get; private init; }

    [Required]
    public string Secret { get; private init; }

    public static JwtOptions Create(string audience, string issuer, int seconds, string secret) => new()
    {
        Audience = audience,
        Issuer = issuer,
        Seconds = seconds,
        Secret = secret
    };
}