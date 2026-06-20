using FinanceService.BackgroundWorker.Models;
using FinanceService.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.BackgroundWorker.Services;

public sealed class ExchangeRateDbUpdater
{
    private readonly IDbContextFactory<FinanceDbContext> _dbContextFactory;
    private readonly ILogger<ExchangeRateDbUpdater> _logger;

    public ExchangeRateDbUpdater(IDbContextFactory<FinanceDbContext> dbContextFactory, ILogger<ExchangeRateDbUpdater> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task Update(ValCurs valCurs, CancellationToken cancellationToken = default)
    {
        var actualCurrencies = valCurs.Valute.Select(Mapper.ToCurrency).ToList();

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var dbCurrencies = await dbContext.Currencies.ToListAsync(cancellationToken);

        var newCurrencies = actualCurrencies
            .Where(ac => dbCurrencies.All(dc => dc.Id != ac.Id));

        var deletedCurrencies = dbCurrencies
            .Where(dc => actualCurrencies.Select(ac => ac.Id).All(id => id != dc.Id))
            .ToList();
        deletedCurrencies.ForEach(c => c.IsDeleted = true);

        foreach (var currency in dbCurrencies.Except(deletedCurrencies))
        {
            currency.Rate = actualCurrencies.First(c => c.Id == currency.Id).Rate;
            currency.IsDeleted = false;
        }

        dbContext.Currencies.AddRange(newCurrencies);

        var changes = dbContext.ChangeTracker.Entries().Select(e => e.State).ToList();

        await dbContext.SaveChangesAsync(cancellationToken);

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Currency updated successfully: added {Added}, modified {Modified}",
                changes.Count(c => c == EntityState.Added),
                changes.Count(c => c == EntityState.Modified));
    }
}