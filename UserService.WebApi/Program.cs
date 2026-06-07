using System.Reflection;
using Database.Core;
using Api.Common;
using Api.Common.Application.Services;
using Api.Common.Domain.Interfaces;
using Api.Common.Presentation.Exceptions;
using UserService.WebApi.Domain.Interfaces;
using UserService.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddAuthorization()
    .AddExchangeRateData(builder.Configuration)
    .AddSingleton<IJwtTokenService, JwtTokenService>()
    .AddSingleton<IUserRepository, UserRepository>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails()
    .AddControllers();

var app = builder.Build();

app.UseSwaggerForDevelopment("User");

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();