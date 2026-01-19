using System.IO.Compression;
using gop.Config;
using gop.Data;
using gop.Extensions;
using gop.Infrastructure;
using gop.Interfaces;
using gop.Repositories;
using gop.Repositories.AdminRepos;
using gop.Repositories.GeneralRepos;
using gop.Security.CurrentUser;
using gop.Seeds;
using gop.Services;
using gop.Services.AdminServices;
using gop.Services.Public;
using gop.Services.Publisher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .Configure<GzipCompressionProviderOptions>(compressionOptions => compressionOptions.Level = CompressionLevel.Fastest)
    .Configure<MvcNewtonsoftJsonOptions>(jsonOptions => jsonOptions.SerializerSettings.Configure())
    .Configure<RouteOptions>(routeOptions => routeOptions.LowercaseUrls = true);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient()
    .AddHttpContextAccessor()
    .AddResponseCompression(compressionOptions =>
    {
        compressionOptions.EnableForHttps = false;
        compressionOptions.Providers.Add<GzipCompressionProvider>();
    })
    .AddApiVersioning(versioningOptions =>
    {
        versioningOptions.DefaultApiVersion = ApiVersion.Default;
        versioningOptions.ReportApiVersions = true;
        versioningOptions.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddVersionedApiExplorer(explorerOptions =>
    {
        explorerOptions.GroupNameFormat = "'v'VVV";
        explorerOptions.SubstituteApiVersionInUrl = true;
    })
    .AddOpenApi()
    .ConfigureAppSettings()
    .AddJwtBearer(builder.Configuration, builder.Environment.IsProduction())
    .AddDatabaseContext()
    .AddInfrastructure();
    
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressMapClientErrors = true;
        options.SuppressModelStateInvalidFilter = false;
    }).AddNewtonsoftJson((_) => { });
    
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAllOrigin", x =>
    {
        x.AllowAnyHeader();
        x.AllowAnyMethod();
        x.WithOrigins("http://localhost:3000", "http://localhost:4200", "http://localhost:3000", "http://localhost:8081")
            .AllowCredentials();
    });
});

builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHashService, BCryptHashService>();
builder.Services.AddScoped<ITokenClaimsService, JwtClaimService>();
// builder.Services.AddScoped<IUserRepository, UserRepository>();

// Repos
builder.Services.AddScoped<ILogsRepo, LogsRepo>();
builder.Services.AddScoped<INoticeRepo, NoticeRepo>();
builder.Services.AddScoped<IUserRepo, UserRepo>();

// services
builder.Services.AddScoped<IAdminAuditAndLogService, AdminAuditAndLogService>();
builder.Services.AddScoped<INoticeService, NoticeService>();
builder.Services.AddScoped<IAdminPublishedService, AdminPublishedService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();

builder.Services.AddScoped<IAuthAccessService, AuthAccessService>();
builder.Services.AddScoped<ISroNumberService, SroNumberService>();
builder.Services.AddScoped<IActReferenceRepo, ActReferenceRepo>();
builder.Services.AddScoped<INotificationRepo, NotificationRepo>();
builder.Services.AddScoped<IPublisherNotificationService, PublisherNotificationService>();
builder.Services.AddScoped<IAdminAnalyticsService, AdminAnalyticsService>();
builder.Services.AddScoped<IAdminAnalyticsRepo, AdminAnalyticsRepo>();
builder.Services.AddScoped<IPublicNoticeService, PublicNoticeService>();
builder.Services.AddScoped<IPublicNoticeRepo, PublicNoticeRepo>();
builder.Services.AddScoped<IPublicVerifySroService, PublicVerifySroService>();
builder.Services.AddScoped<ISroVerificationRepo, SroVerificationRepo>();

var app = builder.Build();

app.UseSwaggerAndUI(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());

app.UseCors("AllowAllOrigin");
app.UseHttpsRedirection();
app.MapControllers();

#region -- Migration + Database

await using var serviceScope = app.Services.CreateAsyncScope();
await using var context = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
try
{
    var connectionString = context.Database.GetConnectionString();
    app.Logger.LogInformation("----- SQL Server: {Connection}", connectionString);
    app.Logger.LogInformation("----- SQL Server: Checking for pending migrations....");

    if ((await context.Database.GetPendingMigrationsAsync()).Any())
    {
        app.Logger.LogInformation("----- SQL Server: Creating and migrating the database....");
        await context.Database.MigrateAsync();
        await AdminAccountSeeder.SeedAsync(app.Services);
        app.Logger.LogInformation("----- SQL Server: Database created and migrated successfully.!");
    }
    else
    {
        app.Logger.LogInformation("----- SQL Server: Migrations are up to date.");
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "An exception occurred while starting the application.: {Message}", ex.Message);
    throw;
}

#endregion

app.Run();