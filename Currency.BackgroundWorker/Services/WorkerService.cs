using Currency.BackgroundWorker.Models;
using Microsoft.Extensions.Options;

namespace Currency.BackgroundWorker.Services;

public sealed class WorkerService : BackgroundService
{
    private readonly XmlDataService _xmlDataService;
    private readonly ExchangeRateDbUpdater _exchangeRateDbUpdater;
    private readonly ExchangeRateSettings _settings;
    private readonly ILogger<WorkerService> _logger;

    public WorkerService(
        XmlDataService xmlDataService,
        ExchangeRateDbUpdater exchangeRateDbUpdater,
        IOptions<ExchangeRateSettings> settings,
        ILogger<WorkerService> logger)
    {
        _xmlDataService = xmlDataService;
        _exchangeRateDbUpdater = exchangeRateDbUpdater;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var valCurs = await _xmlDataService.GetValCursAsync(_settings.CbrCurrencyXmlUrl, stoppingToken);
                await _exchangeRateDbUpdater.Update(valCurs, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred during currency update.");
            }

            await Task.Delay(_settings.UpdateIntervalMs, stoppingToken);
        }
    }
}