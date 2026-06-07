using Database.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Core.DbContexts;

public sealed class JwtDbContext : DbContext
{
    public JwtDbContext()
    { }

    public JwtDbContext(DbContextOptions<JwtDbContext> options)
        : base(options)
    { }

    public DbSet<BlackListToken> BlackListTokens { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("test");
    }
}