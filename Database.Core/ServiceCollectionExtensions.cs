using Database.Core.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExchangeRateData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ExchangeRateDB");
        services.AddDbContextFactory<ExchangeRateDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddJwtData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ExchangeRateDB");
        services.AddDbContextFactory<JwtDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}