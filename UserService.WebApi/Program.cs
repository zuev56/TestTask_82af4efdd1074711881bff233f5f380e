using Shared.Infrastructure;
using Shared.Presentation;
using Shared.Presentation.Exceptions;
using UserService.Application.Interfaces;
using UserService.Application.Services;
using UserService.Infrastructure;
using UserService.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddAuthorization()
    .AddMediatR(config => config.RegisterServicesFromAssembly(typeof(UserService.Application.DTO.RegisterRequest).Assembly))
    .AddIdentityData(builder.Configuration)
    .AddSingleton<IJwtService, JwtService>()
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