using MediatR;
using Microsoft.AspNetCore.Identity;
using Api.Common.Domain.Entities;
using Api.Common.Domain.Interfaces;
using UserService.WebApi.Application.DTO;
using UserService.WebApi.Domain.Interfaces;

namespace UserService.WebApi.Application.Features.Auth.Commands;

public sealed record LoginUserCommand(string Name, string Password) : IRequest<AuthResponse>;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByNameAsync(request.Name, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var passwordCheck = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordCheck == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = _jwtTokenService.GenerateToken(user);
        var validTo = _jwtTokenService.GetExpirationTime(token);

        return new AuthResponse(token, validTo, user.Name, user.Id);
    }
}