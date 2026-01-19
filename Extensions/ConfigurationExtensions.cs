using gop.Options;

namespace gop.Extensions;

/// <summary>
/// Configuration Extensions
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Get Options
    /// </summary>
    /// <param name="configuration"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration)
        where TOptions : class, IAppOptions
    {
        return configuration
            .GetRequiredSection(TOptions.ConfigSectionPath)
            .Get<TOptions>(binderOptions => binderOptions.BindNonPublicProperties = true);
    }
}