using FinanceService.Domain.Entities;

namespace FinanceService.Application.DTO;

public sealed record UserCurrenciesResponse(int UserId, IReadOnlyList<Currency> Currencies);
public sealed record AllCurrenciesResponse(IReadOnlyList<Currency> Currencies);