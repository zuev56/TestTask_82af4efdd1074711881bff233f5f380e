using FinanceService.Application.Interfaces;
using FinanceService.Infrastructure.Persistence.DbContexts;
using FinanceService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Exceptions;
using Currency = FinanceService.Domain.Entities.Currency;

namespace FinanceService.Infrastructure.Persistence.Repositories;

public sealed class CurrencyRepository : ICurrencyRepository
{
    private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;

    public CurrencyRepository(IDbContextFactory<FinanceDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await dbContext.Currencies.AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => new Currency { Id = c.Id, Name = c.Name, Rate = c.Rate })
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Currency>> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var currencyIds = dbContext.UserCurrencies
            .Where(f => f.UserId == userId)
            .Select(f => f.CurrencyId);

        return await dbContext.Currencies.AsNoTracking()
            .Where(c => !c.IsDeleted && currencyIds.Contains(c.Id))
            .Select(c => new Currency { Id = c.Id, Name = c.Name, Rate = c.Rate })
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task AttachToUserAsync(int currencyId, int userId, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (!await dbContext.Currencies.AnyAsync(c => !c.IsDeleted && c.Id == currencyId, cancellationToken))
            throw new NotFoundException($"Currency with id='{currencyId}' not found.");

        if (await dbContext.UserCurrencies.AnyAsync(f => f.UserId == userId && f.CurrencyId == currencyId, cancellationToken: cancellationToken))
            throw new ConflictException($"Currency with id='{currencyId}' already attached to user with id='{userId}'.");

        dbContext.UserCurrencies.Add(new UserCurrency { UserId = userId, CurrencyId = currencyId });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DetachFromUserAsync(int currencyId, int userId, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (!await dbContext.Currencies.AnyAsync(c => c.Id == currencyId, cancellationToken))
            throw new NotFoundException($"Currency with id='{currencyId}' not found.");

        var userFavorite = await dbContext.UserCurrencies
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CurrencyId == currencyId, cancellationToken);
        if (userFavorite == null)
            throw new ArgumentException($"Currency with id='{currencyId}' not attached to user with id='{userId}'.");

        dbContext.UserCurrencies.Remove(userFavorite);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}