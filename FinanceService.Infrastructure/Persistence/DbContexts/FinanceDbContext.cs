using FinanceService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence.DbContexts;

public sealed class FinanceDbContext : DbContext
{
    public FinanceDbContext()
    { }

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
    { }

    public DbSet<Currency> Currencies { get; init; }
    public DbSet<UserCurrency> UserCurrencies { get; init; }
}