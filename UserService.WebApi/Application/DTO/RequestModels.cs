namespace UserService.WebApi.Application.DTO;

public sealed record RegisterRequest(string Name, string Password);
public sealed record LoginRequest(string Name, string Password);
public sealed record UserResponse(int Id, string Name);
public sealed record AuthResponse(string Token, DateTime ValidTo, string UserName, int UserId);