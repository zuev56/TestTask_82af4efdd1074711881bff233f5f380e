using FinanceService.Application.DTO;
using FinanceService.Application.Interfaces;
using MediatR;

namespace FinanceService.Application.Features.Currency.Queries;

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