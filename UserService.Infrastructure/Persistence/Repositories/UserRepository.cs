using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using UserService.Application.Interfaces;
using UserService.Infrastructure.Persistence.DbContexts;
using DbUser = UserService.Infrastructure.Persistence.Entities.User;

namespace UserService.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<IdentityDbContext> _dbContextFactory;

    public UserRepository(IDbContextFactory<IdentityDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Users.AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new User { Id = u.Id, Name = u.Name, PasswordHash = u.PasswordHash })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Users.AsNoTracking()
            .Where(u => u.Name == name)
            .Select(u => new User { Id = u.Id, Name = u.Name, PasswordHash = u.PasswordHash })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var dbUser = new DbUser {Name = user.Name, PasswordHash = user.PasswordHash};
        dbContext.Users.Add(dbUser);
        await dbContext.SaveChangesAsync(cancellationToken);

        user.Id = dbUser.Id;
    }
}