using CurrencyService.WebApi.Domain.Entities;

namespace CurrencyService.WebApi.Application.DTO;

public sealed record UserCurrenciesResponse(int UserId, IReadOnlyList<Currency> Currencies);
public sealed record AllCurrenciesResponse(IReadOnlyList<Currency> Currencies);