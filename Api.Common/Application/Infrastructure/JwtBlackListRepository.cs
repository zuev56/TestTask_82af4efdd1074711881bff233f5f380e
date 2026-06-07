using Api.Common.Domain.Interfaces;
using Database.Core.DbContexts;
using Database.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api.Common.Application.Infrastructure;

public sealed class JwtBlackListRepository : IJwtBlackListRepository
{
    private readonly IDbContextFactory<JwtDbContext> _dbContextFactory;

    public JwtBlackListRepository(IDbContextFactory<JwtDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddAsync(string token, DateTime expiration, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        dbContext.BlackListTokens.Add(new BlackListToken {Token = token, ExpirationDate = expiration});
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ContainsAsync(string token, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.BlackListTokens.AnyAsync(t => t.Token == token, cancellationToken: cancellationToken);
    }

    // TODO: Предполагается автоудаление протухших токенов либо средствами Postgres, либо отдельным воркером.
    //       А лучше перенести хранение токунов в Redis.
    public async Task RemoveExpiredAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.BlackListTokens
            .Where(t => t.ExpirationDate < DateTime.UtcNow)
            .ExecuteDeleteAsync(cancellationToken);
    }
}