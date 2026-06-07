using Api.Common.Domain.Exceptions;
using CurrencyService.WebApi.Domain.Entities;
using CurrencyService.WebApi.Domain.Interfaces;
using Database.Core.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CurrencyService.WebApi.Infrastructure;

public sealed class CurrencyRepository : ICurrencyRepository
{
    private readonly IDbContextFactory<ExchangeRateDbContext> _dbContextFactory;

    public CurrencyRepository(IDbContextFactory<ExchangeRateDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Currencies.AsNoTracking()
            .Select(c => new Currency { Id = c.Id, Name = c.Name, Rate = c.Rate })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Currency>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Currencies.Select(c => new Currency { Id = c.Id, Name = c.Name, Rate = c.Rate }))
            .ToListAsync(cancellationToken);
    }

    public async Task AttachToUserAsync(int currencyId, int userId, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var currency = await dbContext.Currencies.FirstOrDefaultAsync(c => c.Id == currencyId, cancellationToken);
        if (currency == null)
            throw new NotFoundException($"Currency with id='{currencyId}' not found.");

        var user = await dbContext.Users
            .Include(u => u.Currencies)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new NotFoundException($"User with id='{userId}' not found.");

        if (user.Currencies.Any(c => c.Id == currencyId))
            throw new ConflictException($"Currency with id='{currencyId}' already attached to user with id='{userId}'.");

        user.Currencies.Add(currency);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DetachFromUserAsync(int currencyId, int userId, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var currency = await dbContext.Currencies.FirstOrDefaultAsync(c => c.Id == currencyId, cancellationToken);
        if (currency == null)
            throw new NotFoundException($"Currency with id='{currencyId}' not found.");

        var user = await dbContext.Users
            .Include(u => u.Currencies)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
            throw new NotFoundException($"User with id='{userId}' not found.");

        if (user.Currencies.All(c => c.Id != currencyId))
            throw new ArgumentException($"Currency with id='{currencyId}' not attached to user with id='{userId}'.");

        user.Currencies.Remove(currency);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}