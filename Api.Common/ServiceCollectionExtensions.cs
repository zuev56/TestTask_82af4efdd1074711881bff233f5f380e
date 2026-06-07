using Api.Common.Application.Infrastructure;
using Api.Common.Application.Models;
using Api.Common.Domain.Entities;
using Api.Common.Domain.Interfaces;
using Database.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Api.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddJwtData(configuration)
            .AddSingleton<IJwtBlackListRepository, JwtBlackListRepository>()
            .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            throw new InvalidOperationException("Jwt.SecretKey is missing");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwtSettings.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = async context =>
                    {
                        var accessToken = EnvironmentUtils.IsDevelopment()
                            ? context.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "")
                            : context.Request.Cookies["Authorization"];

                        if (string.IsNullOrWhiteSpace(accessToken))
                            return;

                        var serviceProvider = context.HttpContext.RequestServices;
                        var jwtBlackListRepository = serviceProvider.GetRequiredService<IJwtBlackListRepository>();
                        if (await jwtBlackListRepository.ContainsAsync(accessToken))
                            return;

                        context.Token = accessToken;
                    }
                };
            });

        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            var openApiReference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"};
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme { Reference = openApiReference }, [] }
            });
        });

        return services;
    }
}