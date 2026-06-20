using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestProperties |
                            HttpLoggingFields.ResponsePropertiesAndHeaders;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/finance/swagger/v1/swagger.json", "Finance API");
        options.SwaggerEndpoint("/user/swagger/v1/swagger.json", "User API");

        options.EnableTryItOutByDefault();
    });
}

app.UseHttpLogging();
app.MapReverseProxy();

app.Run();