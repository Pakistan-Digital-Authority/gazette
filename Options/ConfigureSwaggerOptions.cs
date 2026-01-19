using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace gop.Options;

public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var openApiInfo = new OpenApiInfo
        {
            Title = "Gazette of Pakistan - API's - Version 1.0.0",
            Description = "ASP.NET Core Rest API for Gazette of Pakistan - a web/mobile based application",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact { Name = "Shakil Ahmad Khan", Email = "shakilak196@gmail.com" }
        };

        if (description.IsDeprecated)
            openApiInfo.Description += " -------- This version of the API has been discontinued!";

        return openApiInfo;
    }
}