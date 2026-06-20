using FinanceService.Application.Interfaces;
using MediatR;

namespace FinanceService.Application.Features.Currency.Commands;

public sealed record AttachCurrencyToUserCommand(int CurrencyId, int UserId) : IRequest;

public class AttachCurrencyToUserCommandHandler : IRequestHandler<AttachCurrencyToUserCommand>
{
    private readonly ICurrencyRepository _currencyRepository;

    public AttachCurrencyToUserCommandHandler(ICurrencyRepository currencyRepository)
    {
        _currencyRepository = currencyRepository;
    }

    public Task Handle(AttachCurrencyToUserCommand request, CancellationToken cancellationToken)
        => _currencyRepository.AttachToUserAsync(request.CurrencyId, request.UserId, cancellationToken);
}