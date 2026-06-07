using System.ComponentModel.DataAnnotations;

namespace Currency.BackgroundWorker.Models;

public sealed class ExchangeRateSettings
{
    public const string SectionName = "ExchangeRate";

    [Required]
    public required string CbrCurrencyXmlUrl { get; init; }
    [Required]
    public required int UpdateIntervalMs { get; init; }
    [Required]
    public required int RetryCount { get; init; }
}