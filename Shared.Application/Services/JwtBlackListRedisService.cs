using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Application.Interfaces;

namespace Shared.Application.Services;

public sealed class JwtBlackListRedisService : IJwtBlackListService
{
    private readonly IDistributedCache _cache;

    public JwtBlackListRedisService(IDistributedCache distributedCache)
    {
        _cache = distributedCache;
    }

    public async Task AddToBlackListAsync(string token, DateTime expirationDate, CancellationToken cancellationToken)
    {
        var ttl = expirationDate - DateTimeOffset.UtcNow;
        if (ttl <= TimeSpan.Zero)
            return;

        var key = GetKey(token);
        await _cache.SetStringAsync(key, "revoked", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        }, cancellationToken);
    }

    public async Task<bool> IsBlackListedAsync(string token, CancellationToken cancellationToken)
    {
        var key = GetKey(token);
        var value = await _cache.GetStringAsync(key, cancellationToken);

        return value != null;
    }

    private static string GetKey(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        var hash = Convert.ToBase64String(bytes);

        return $"blacklist:{hash}";
    }
}