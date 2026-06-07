using System.Reflection;
using CurrencyService.WebApi.Domain.Interfaces;
using CurrencyService.WebApi.Infrastructure;
using Database.Core;
using Api.Common;
using Api.Common.Presentation.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddAuthorization()
    .AddExchangeRateData(builder.Configuration)
    .AddSingleton<ICurrencyRepository, CurrencyRepository>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails()
    .AddControllers();

var app = builder.Build();

app.UseSwaggerForDevelopment("Currency");

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();