using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Persistence.Entities;

namespace UserService.Infrastructure.Persistence.DbContexts;

public sealed class IdentityDbContext : DbContext
{
    public IdentityDbContext()
    { }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    { }

    public DbSet<User> Users { get; init; }
}