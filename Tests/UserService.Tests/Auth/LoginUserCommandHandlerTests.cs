using Api.Common.Domain.Entities;
using Api.Common.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using UserService.WebApi.Application.Features.Auth.Commands;
using UserService.WebApi.Domain.Interfaces;
using Xunit;

namespace UserService.Tests.Auth;

public sealed class LoginUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher<User>>();
        _jwtTokenService = Substitute.For<IJwtTokenService>();
    }

    [Fact]
    public async Task Handle_LoginUserCommand_WhenUserNotFound_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var userName = "user1";
        var password = "pass1";
        var command = new LoginUserCommand(userName, password);
        var handler = new LoginUserCommandHandler(_userRepository, _passwordHasher, _jwtTokenService);
        _userRepository.GetByNameAsync(userName, CancellationToken.None)
            .Returns(Task.FromResult<User?>(null));

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        _jwtTokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task Handle_LoginUserCommand_WhenPasswordNotValid_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var userName = "user1";
        var password = "pass1";
        var hashedPassword = "pass1Hash";
        var user = new User { Id = 1, Name = userName, PasswordHash = hashedPassword };
        var command = new LoginUserCommand(userName, password);
        var handler = new LoginUserCommandHandler(_userRepository, _passwordHasher, _jwtTokenService);
        _userRepository.GetByNameAsync(userName, CancellationToken.None)
            .Returns(Task.FromResult<User?>(null));
        _passwordHasher.VerifyHashedPassword(user, hashedPassword, password)
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        _jwtTokenService.DidNotReceive().GenerateToken(Arg.Any<User>());
        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task Handle_ValidUserAndPassword_ShouldReturnAuthResponse()
    {
        // Arrange
        var userName = "user1";
        var password = "pass1";
        var hashedPassword = "pass1Hash";
        var token = "token";
        var expirationTime = DateTime.UtcNow.AddHours(1);
        var user = new User { Id = 1, Name = userName, PasswordHash = hashedPassword };
        var command = new LoginUserCommand(userName, password);
        var handler = new LoginUserCommandHandler(_userRepository, _passwordHasher, _jwtTokenService);
        _userRepository.GetByNameAsync(userName, CancellationToken.None)
            .Returns(user);
        _passwordHasher.VerifyHashedPassword(user, hashedPassword, password)
            .Returns(PasswordVerificationResult.Success);
        _jwtTokenService.GenerateToken(user)
            .Returns(token);
        _jwtTokenService.GetExpirationTime(token)
            .Returns(expirationTime);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        _jwtTokenService.Received(1).GenerateToken(user);
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
        result.ValidTo.Should().Be(expirationTime);
        result.UserId.Should().Be(user.Id);
        result.UserName.Should().Be(user.Name);
    }
}