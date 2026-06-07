using Currency.BackgroundWorker.Models;

namespace Currency.BackgroundWorker.Services;

public static class Mapper
{
    public static Database.Core.Entities.Currency ToCurrency(Valute valute)
        => new Database.Core.Entities.Currency
        {
            Id = valute.NumCode,
            Name = valute.Name,
            Rate = valute.VunitRate
        };
}