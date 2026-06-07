using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Api.Common;

public static class WebApplicationExtensions
{
    public static void UseSwaggerForDevelopment(this WebApplication app, string serviceName)
    {
        if (!app.Environment.IsDevelopment())
            return;

        app.UseSwagger(options =>
        {
            options.RouteTemplate = $"{serviceName}/swagger/{{documentName}}/swagger.json";
        });
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("v1/swagger.json", $"{serviceName} API");
            options.RoutePrefix = $"{serviceName}/swagger";
            options.EnableTryItOutByDefault();
        });
    }
}