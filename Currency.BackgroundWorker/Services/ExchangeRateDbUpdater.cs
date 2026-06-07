using Currency.BackgroundWorker.Models;
using Database.Core.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Currency.BackgroundWorker.Services;

public sealed class ExchangeRateDbUpdater
{
    private readonly IDbContextFactory<ExchangeRateDbContext> _dbContextFactory;
    private readonly ILogger<ExchangeRateDbUpdater> _logger;

    public ExchangeRateDbUpdater(IDbContextFactory<ExchangeRateDbContext> dbContextFactory, ILogger<ExchangeRateDbUpdater> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public async Task Update(ValCurs valCurs, CancellationToken cancellationToken = default)
    {
        // Полагаю, всегда требуется только актуальные данные.
        // Если какая-то валюта пропала из списка, значит её не должно быть и в БД.
        // Т.к. объём данных небольшой, можно не заморачиваться на обновление актуальных значений и удаление только пропавших из списка.
        // TODO: А лучше реализовать логическое удаление, чтобы не рушилась связь валюты с пользователем, если останется время.

        var newCurrencies = valCurs.Valute.Select(Mapper.ToCurrency);

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        await dbContext.Currencies.ExecuteDeleteAsync(cancellationToken);

        dbContext.Currencies.AddRange(newCurrencies);

        await dbContext.SaveChangesAsync(cancellationToken);

        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("Currency updated successfully.");
    }
}