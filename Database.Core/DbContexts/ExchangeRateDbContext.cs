using Database.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Core.DbContexts;

public sealed class ExchangeRateDbContext : DbContext
{
    public ExchangeRateDbContext()
    { }

    public ExchangeRateDbContext(DbContextOptions<ExchangeRateDbContext> options)
        : base(options)
    { }

    public DbSet<Currency> Currencies { get; init; }
    public DbSet<User> Users { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("test");
    }
}