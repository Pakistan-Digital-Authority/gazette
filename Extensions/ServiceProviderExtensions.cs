using gop.Options;
using Microsoft.Extensions.Options;

namespace gop.Extensions;

/// <summary>
/// Swagger Service Provider
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Get Options
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <typeparam name="TOptions"></typeparam>
    /// <returns></returns>
    public static TOptions GetOptions<TOptions>(this IServiceProvider serviceProvider)
        where TOptions : class, IAppOptions => serviceProvider.GetRequiredService<IOptions<TOptions>>().Value;
}