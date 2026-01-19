using gop.Infrastructure.EmailService;
using gop.Infrastructure.Storage;
using gop.Interfaces;
using gop.Services;

namespace gop.Infrastructure;

/// <summary>
/// Dependency injection for infra
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// To add the infra dependencies
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IEmailSender, MimeKitEmailSender>();
        services.AddScoped<IFileStorageService, LocalStorageService>();

        return services;
    }
}