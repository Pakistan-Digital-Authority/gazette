using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using gop.Options;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace gop.Extensions;

[ExcludeFromCodeCoverage]
internal static class SwaggerExtensions
{
    internal static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(swaggerOptions =>
        {
            swaggerOptions.OperationFilter<SwaggerDefaultValuesFilter>();

            swaggerOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization Header - used with Bearer Authentication.\r\n\r\n" +
                    "Enter 'Bearer' [space] and then your token in the field below.\r\n\r\n" +
                    "Example (provide without quotation marks): 'Bearer token-here'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            swaggerOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            swaggerOptions.ResolveConflictingActions(apiDescription => apiDescription.FirstOrDefault());

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swaggerOptions.IncludeXmlComments(xmlPath, true);
        });

        services.AddSwaggerGenNewtonsoftSupport();

        return services;
    }

    internal static void UseSwaggerAndUI(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(swaggerOptions =>
        {
            // Build a swagger endpoint for each discovered API version
            foreach (var groupName in provider
                .ApiVersionDescriptions
                .Select(description => description.GroupName)
                .Distinct()
                .ToList())
            {
                swaggerOptions.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
            }
        });
    }
}