using FinanceService.BackgroundWorker.Models;

namespace FinanceService.BackgroundWorker.Services;

public static class Mapper
{
    public static Infrastructure.Persistence.Entities.Currency ToCurrency(Valute valute)
        => new Infrastructure.Persistence.Entities.Currency
        {
            Id = valute.NumCode,
            Name = valute.Name,
            Rate = valute.VunitRate,
            IsDeleted = false
        };
}