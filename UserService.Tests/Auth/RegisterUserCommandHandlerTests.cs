using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shared.Domain.Entities;
using Shared.Domain.Exceptions;
using UserService.Application.Features.Auth.Commands;
using UserService.Application.Interfaces;
using Xunit;

namespace UserService.Tests.Auth;

public sealed class RegisterUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher<User>>();
    }

    [Fact]
    public async Task Handle_RegisterUserCommand_WhenUserAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        var userName = "user1";
        var password = "pass1";
        var command = new RegisterUserCommand(userName, password);
        var handler = new RegisterUserCommandHandler(_userRepository, _passwordHasher);

        _userRepository.GetByNameAsync(userName, CancellationToken.None)
            .Returns(Task.FromResult<User?>(new User { Name = userName }));

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ConflictException>().WithMessage($"User with name '{userName}' already exists");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_RegisterUserCommand_WhenPasswordIsEmpty_ShouldThrowArgumentException(string? password)
    {
        // Arrange
        var userName = "user1";
        var command = new RegisterUserCommand(userName, password!);
        var handler = new RegisterUserCommandHandler(_userRepository, _passwordHasher);

        _userRepository.GetByNameAsync(userName, CancellationToken.None)
            .Returns(Task.FromResult<User?>(null));

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>().WithMessage("Password is required");
    }

    [Fact]
    public async Task Handle__RegisterUserCommand_WhenNewUserAndValidPassword_ShouldReturnUserResponse()
    {
        // Arrange
        var userName = "user1";
        var password = "pass1";
        var hashedPassword = "hashedPassword";
        var command = new RegisterUserCommand(userName, password);
        var handler = new RegisterUserCommandHandler(_userRepository, _passwordHasher);

        _userRepository.GetByNameAsync(userName, CancellationToken.None)
            .Returns(Task.FromResult<User?>(null));

        _passwordHasher.HashPassword(new User { Name = userName }, password)
            .Returns(hashedPassword);

        var user = new User { Name = userName };
        _userRepository.AddAsync(Arg.Is(user), CancellationToken.None)
            .Returns(Task.FromResult(user));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), CancellationToken.None);
        result.Should().NotBeNull();
        result.Name.Should().Be(userName);
    }
}