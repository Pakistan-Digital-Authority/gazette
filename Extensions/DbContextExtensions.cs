using System.Diagnostics.CodeAnalysis;
using gop.Config;
using gop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace gop.Extensions;

[ExcludeFromCodeCoverage]
internal static class DbContextExtensions
{
    private const int DbMaxRetryCount = 3;
    private const int DbCommandTimeout = 35;
    private const string DbInMemoryName = $"Db-InMemory-{nameof(DatabaseContext)}";
    private const string DbMigrationAssemblyName = "gop";

    private static readonly string[] DbRelationalTags = ["database", "ef-core", "sql-server", "relational"];

    internal static IServiceCollection AddDatabaseContext(this IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>((serviceProvider, optionsBuilder) =>
        {
            var connections = serviceProvider.GetOptions<ConnectionStrings>();

            optionsBuilder.UseSqlServer(connections.Database, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly(DbMigrationAssemblyName);
                sqlServerOptions.EnableRetryOnFailure(DbMaxRetryCount);
                sqlServerOptions.CommandTimeout(DbCommandTimeout);
            });

            var logger = serviceProvider.GetRequiredService<ILogger<DatabaseContext>>();

            //Log retry attempts.
            optionsBuilder.LogTo(
                (eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
                eventData =>
                {
                    if (eventData is not ExecutionStrategyEventData retryEventData)
                        return;

                    var exceptions = retryEventData.ExceptionsEncountered;

                    logger.LogWarning(
                        "----- Retry #{Count} with delay {Delay} due to error: {Message}",
                        exceptions.Count,
                        retryEventData.Delay,
                        exceptions[^1].Message);
                });

            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (!environment.IsProduction())
            {
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
            }
        });

        return services;
    }
}