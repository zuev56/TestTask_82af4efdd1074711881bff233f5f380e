using System.Net;
using System.Text;
using FinanceService.Infrastructure;
using FinanceService.BackgroundWorker.Models;
using FinanceService.BackgroundWorker.Services;
using Polly;
using Polly.Extensions.Http;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOptions<ExchangeRateSettings>()
    .Bind(builder.Configuration.GetSection(ExchangeRateSettings.SectionName))
    .ValidateOnStart();

var exchangeRateSettings = builder.Configuration
    .GetSection(ExchangeRateSettings.SectionName)
    .Get<ExchangeRateSettings>()!;

builder.Services
    .AddHostedService<WorkerService>()
    .AddFinanceData(builder.Configuration)
    .AddSingleton<XmlDataService>()
    .AddSingleton<ExchangeRateDbUpdater>()
    .AddHttpClient("CurrencyUpdater", client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
        client.DefaultRequestHeaders.Add("Accept", "application/xml, text/xml");
    })
    .AddPolicyHandler((serviceProvider, _) =>
    {
        var logger = serviceProvider.GetRequiredService<ILogger<WorkerService>>();
        return GetRetryPolicy(exchangeRateSettings.RetryCount, logger);
    });


var host = builder.Build();
host.Run();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount, ILogger logger)
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(x => x.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(
            retryCount: retryCount,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timeSpan, retryAttempt, _) =>
            {
                logger.LogWarning(outcome.Exception,
                    "Repeat attempt {RetryAttempt} after {TimeSpanTotalSeconds} seconds. Error: {error}",
                    retryAttempt, timeSpan.TotalSeconds, outcome.Exception.Message);
            });