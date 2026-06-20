using Shared.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpirationTime(string token);
}