using FluentAssertions;
using NSubstitute;
using Shared.Application.Interfaces;
using UserService.Application.Features.Auth.Commands;
using UserService.Application.Interfaces;
using Xunit;

namespace UserService.Tests.Auth;

public sealed class LogoutUserCommandHandlerTests
{
    private readonly IJwtService _jwtTokenService;
    private readonly IJwtBlackListService _jwtBlackListService;

    public LogoutUserCommandHandlerTests()
    {
        _jwtTokenService = Substitute.For<IJwtService>();
        _jwtBlackListService = Substitute.For<IJwtBlackListService>();
    }

    [Fact]
    public async Task Handle_LogoutUserCommand_WhenTokenIsExpired_ShouldNotAddTokenToBlackList()
    {
        // Arrange
        var token = "token";
        var command = new LogoutUserCommand(token);
        var handler = new LogoutUserCommandHandler(_jwtTokenService, _jwtBlackListService);

        _jwtTokenService.GetExpirationTime(token)
            .Returns(DateTime.UtcNow.AddHours(-1));

        // Act
        var result = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await result.Should().NotThrowAsync();
        await _jwtBlackListService.DidNotReceive().IsBlackListedAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _jwtBlackListService.DidNotReceive().AddToBlackListAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LogoutUserCommand_WhenValidTokenAlreadyInBlackList_ShouldNotAddTokenToBlackList()
    {
        // Arrange
        var token = "token";
        var command = new LogoutUserCommand(token);
        var handler = new LogoutUserCommandHandler(_jwtTokenService, _jwtBlackListService);

        _jwtTokenService.GetExpirationTime(token)
            .Returns(DateTime.UtcNow.AddHours(1));
        _jwtBlackListService.IsBlackListedAsync(token, CancellationToken.None)
            .Returns(Task.FromResult(true));

        // Act
        var result = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await result.Should().NotThrowAsync();
        await _jwtBlackListService.Received().IsBlackListedAsync(token, Arg.Any<CancellationToken>());
        await _jwtBlackListService.DidNotReceive().AddToBlackListAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LogoutUserCommand_WhenValidTokenNotInBlackList_ShouldAddTokenToBlackList()
    {
        // Arrange
        var token = "token";
        var expirationTime = DateTime.UtcNow.AddHours(1);
        var command = new LogoutUserCommand(token);
        var handler = new LogoutUserCommandHandler(_jwtTokenService, _jwtBlackListService);

        _jwtTokenService.GetExpirationTime(token)
            .Returns(expirationTime);
        _jwtBlackListService.IsBlackListedAsync(token, CancellationToken.None)
            .Returns(Task.FromResult(false));

        // Act
        var result = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await result.Should().NotThrowAsync();
        await _jwtBlackListService.Received().IsBlackListedAsync(token, Arg.Any<CancellationToken>());
        await _jwtBlackListService.Received().AddToBlackListAsync(token, expirationTime, Arg.Any<CancellationToken>());
    }
}