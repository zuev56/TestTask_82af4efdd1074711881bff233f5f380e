using Api.Common.Domain.Exceptions;
using CurrencyService.WebApi.Application.DTO;
using CurrencyService.WebApi.Domain.Interfaces;
using MediatR;

namespace CurrencyService.WebApi.Application.Features.Currency.Queries;

public sealed record GetUserCurrenciesQuery(int UserId) : IRequest<UserCurrenciesResponse>;

public sealed class GetUserCurrenciesQueryHandler : IRequestHandler<GetUserCurrenciesQuery, UserCurrenciesResponse>
{
    private readonly ICurrencyRepository _currencyRepository;

    public GetUserCurrenciesQueryHandler(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public async Task<UserCurrenciesResponse> Handle(GetUserCurrenciesQuery query, CancellationToken cancellationToken)
    {
        var userCurrencies = await _currencyRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        if (!userCurrencies.Any())
            throw new NotFoundException($"Currencies for user with Id='{query.UserId}' not found.");

        return new UserCurrenciesResponse(query.UserId, userCurrencies);
    }
}