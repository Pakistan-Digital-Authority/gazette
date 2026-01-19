using System.Diagnostics.CodeAnalysis;
using System.Text;
using gop.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace gop.Extensions;

[ExcludeFromCodeCoverage]
internal static class JwtBearerExtensions
{
    internal static IServiceCollection AddJwtBearer(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isProduction)
    {
        var jwtOptions = configuration.GetOptions<JwtOptions>();

        services
            .AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearerOptions =>
            {
                // RequireHttpsMetadata should always be enabled in the production environment.
                bearerOptions.RequireHttpsMetadata = isProduction;
                bearerOptions.SaveToken = true;
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidIssuer = jwtOptions.Issuer
                };
            });

        // Enables the use of tokens as a means to authorize access to resources in this project.
        services.AddAuthorizationBuilder()
            .AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());

        return services;
    }
}