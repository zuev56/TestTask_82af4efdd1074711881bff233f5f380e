namespace Shared.Application.Interfaces;

public interface IJwtBlackListService
{
    public Task AddToBlackListAsync(string token, DateTime expiration, CancellationToken cancellationToken = default);
    public Task<bool> IsBlackListedAsync(string token, CancellationToken cancellationToken = default);
}