using Database.Core.DbContexts;
using Microsoft.EntityFrameworkCore;
using Api.Common.Domain.Entities;
using UserService.WebApi.Domain.Interfaces;
using DbUser = Database.Core.Entities.User;

namespace UserService.WebApi.Infrastructure;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbContextFactory<ExchangeRateDbContext> _dbContextFactory;

    public UserRepository(IDbContextFactory<ExchangeRateDbContext> dbContextFactory)
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