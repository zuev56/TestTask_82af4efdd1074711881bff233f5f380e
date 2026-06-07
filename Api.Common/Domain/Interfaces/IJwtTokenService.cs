using Api.Common.Domain.Entities;

namespace Api.Common.Domain.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    DateTime GetExpirationTime(string token);
}