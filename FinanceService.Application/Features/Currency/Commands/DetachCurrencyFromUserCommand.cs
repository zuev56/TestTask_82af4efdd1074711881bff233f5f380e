using FinanceService.Application.Interfaces;
using MediatR;

namespace FinanceService.Application.Features.Currency.Commands;

public sealed record DetachCurrencyFromUserCommand(int CurrencyId, int UserId) : IRequest;

public class DetachCurrencyFromUserCommandHandler : IRequestHandler<DetachCurrencyFromUserCommand>
{
    private readonly ICurrencyRepository _currencyRepository;

    public DetachCurrencyFromUserCommandHandler(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public Task Handle(DetachCurrencyFromUserCommand request, CancellationToken cancellationToken)
        => _currencyRepository.DetachFromUserAsync(request.CurrencyId, request.UserId, cancellationToken);
}