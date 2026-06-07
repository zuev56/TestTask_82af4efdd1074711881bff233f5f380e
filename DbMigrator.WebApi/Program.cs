using Database.Core;
using Database.Core.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExchangeRateData(builder.Configuration);
builder.Services.AddJwtData(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
}

app.UseHttpsRedirection();

app.MapPost("/migrations/apply",
        async ([FromServices] IDbContextFactory<ExchangeRateDbContext> exchangeRateContextFactory,
            [FromServices] IDbContextFactory<JwtDbContext> jwtContextFactory,
            [FromServices] ILogger<Program> logger) =>
        {
            // TODO: Решение такое себе (миграция только в одну сторону, надо добавлять каждый новый контекст).
            //       Для большей гибкости можно передеать на выполнение команд через CLI.
            try
            {
                await using var exchangeRateContext = await exchangeRateContextFactory.CreateDbContextAsync();
                await exchangeRateContext.Database.MigrateAsync();

                await using var jwtContext = await jwtContextFactory.CreateDbContextAsync();
                await jwtContext.Database.MigrateAsync();

                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("Migrated successfully");

                return Results.Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database");
                return Results.Problem("An error occurred while migrating the database");
            }
        })
    .WithName("ApplyMigrations")
    .WithOpenApi();

app.Run();