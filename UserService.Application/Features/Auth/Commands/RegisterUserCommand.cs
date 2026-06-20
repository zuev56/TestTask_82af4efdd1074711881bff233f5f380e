using MediatR;
using Microsoft.AspNetCore.Identity;
using Shared.Domain.Entities;
using Shared.Domain.Exceptions;
using UserService.Application.DTO;
using UserService.Application.Interfaces;

namespace UserService.Application.Features.Auth.Commands;

public sealed record RegisterUserCommand(string Name, string Password) : IRequest<UserResponse>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingUser != null)
            throw new ConflictException($"User with name '{request.Name}' already exists");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Password is required");

        var user = new User { Name = request.Name };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);

        return new UserResponse(user.Id, user.Name);
    }
}