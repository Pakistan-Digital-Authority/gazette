using System.Diagnostics.CodeAnalysis;
using gop.Options;

namespace gop.Config;

/// <summary>
/// Injecting Services
/// </summary>
[ExcludeFromCodeCoverage]
public static class DependencyInjectionRegister
{
    /// <summary>
    /// Configure App Settings
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureAppSettings(this IServiceCollection services) =>
        services
            .AddOptionsWithValidation<AuthOptions>()
            .AddOptionsWithValidation<ConnectionStrings>()
            .AddOptionsWithValidation<JwtOptions>();

    private static IServiceCollection AddOptionsWithValidation<TOptions>(this IServiceCollection services)
        where TOptions : class, IAppOptions
    {
        return services
            .AddOptions<TOptions>()
            .BindConfiguration(TOptions.ConfigSectionPath, binderOptions => binderOptions.BindNonPublicProperties = true)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .Services;
    }
}