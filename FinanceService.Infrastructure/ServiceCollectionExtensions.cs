using FinanceService.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFinanceData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FinanceDB");
        services.AddDbContextFactory<FinanceDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}