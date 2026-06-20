using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Infrastructure.Persistence.DbContexts;

namespace UserService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IdentityDB");
        services.AddDbContextFactory<IdentityDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}