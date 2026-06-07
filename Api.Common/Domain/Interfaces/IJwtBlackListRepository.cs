namespace Api.Common.Domain.Interfaces;

public interface IJwtBlackListRepository
{
    Task AddAsync(string token, DateTime expiration, CancellationToken cancellationToken = default);
    Task<bool> ContainsAsync(string token, CancellationToken cancellationToken = default);
    Task RemoveExpiredAsync(CancellationToken cancellationToken = default);
}