using FinanceService.Domain.Entities;

namespace FinanceService.Application.Interfaces;

public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Currency>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AttachToUserAsync(int currencyId, int userId, CancellationToken cancellationToken = default);
    Task DetachFromUserAsync(int currencyId, int userId, CancellationToken cancellationToken = default);
}