using CurrencyService.WebApi.Application.DTO;
using CurrencyService.WebApi.Domain.Interfaces;
using MediatR;

namespace CurrencyService.WebApi.Application.Features.Currency.Queries;

public sealed record GetAllCurrenciesQuery : IRequest<AllCurrenciesResponse>;

public sealed class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, AllCurrenciesResponse>
{
    private readonly ICurrencyRepository _currencyRepository;

    public GetAllCurrenciesQueryHandler(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<AllCurrenciesResponse> Handle(GetAllCurrenciesQuery query, CancellationToken cancellationToken)
    {
        var allCurrencies = await _currencyRepository.GetAllAsync(cancellationToken);
        return new AllCurrenciesResponse(allCurrencies);
    }
}