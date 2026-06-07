using Api.Common.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;
using UserService.WebApi.Application.Features.Auth.Commands;
using Xunit;

namespace UserService.Tests.Auth;

public sealed class LogoutUserCommandHandlerTests
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IJwtBlackListRepository _jwtTokenBlackListRepository;

    public LogoutUserCommandHandlerTests()
    {
        _jwtTokenService = Substitute.For<IJwtTokenService>();
        _jwtTokenBlackListRepository = Substitute.For<IJwtBlackListRepository>();
    }

    [Fact]
    public async Task Handle_LogoutUserCommand_WhenTokenIsExpired_ShouldNotAddTokenToBlackList()
    {
        // Arrange
        var token = "token";
        var command = new LogoutUserCommand(token);
        var handler = new LogoutUserCommandHandler(_jwtTokenService, _jwtTokenBlackListRepository);

        _jwtTokenService.GetExpirationTime(token)
            .Returns(DateTime.UtcNow.AddHours(-1));

        // Act
        var result = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await result.Should().NotThrowAsync();
        await _jwtTokenBlackListRepository.DidNotReceive().ContainsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _jwtTokenBlackListRepository.DidNotReceive().AddAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LogoutUserCommand_WhenValidTokenAlreadyInBlackList_ShouldNotAddTokenToBlackList()
    {
        // Arrange
        var token = "token";
        var command = new LogoutUserCommand(token);
        var handler = new LogoutUserCommandHandler(_jwtTokenService, _jwtTokenBlackListRepository);

        _jwtTokenService.GetExpirationTime(token)
            .Returns(DateTime.UtcNow.AddHours(1));
        _jwtTokenBlackListRepository.ContainsAsync(token, CancellationToken.None)
            .Returns(Task.FromResult(true));

        // Act
        var result = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await result.Should().NotThrowAsync();
        await _jwtTokenBlackListRepository.Received().ContainsAsync(token, Arg.Any<CancellationToken>());
        await _jwtTokenBlackListRepository.DidNotReceive().AddAsync(Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LogoutUserCommand_WhenValidTokenNotInBlackList_ShouldAddTokenToBlackList()
    {
        // Arrange
        var token = "token";
        var expirationTime = DateTime.UtcNow.AddHours(1);
        var command = new LogoutUserCommand(token);
        var handler = new LogoutUserCommandHandler(_jwtTokenService, _jwtTokenBlackListRepository);

        _jwtTokenService.GetExpirationTime(token)
            .Returns(expirationTime);
        _jwtTokenBlackListRepository.ContainsAsync(token, CancellationToken.None)
            .Returns(Task.FromResult(false));

        // Act
        var result = () => handler.Handle(command, CancellationToken.None);

        // Assert
        await result.Should().NotThrowAsync();
        await _jwtTokenBlackListRepository.Received().ContainsAsync(token, Arg.Any<CancellationToken>());
        await _jwtTokenBlackListRepository.Received().AddAsync(token, expirationTime, Arg.Any<CancellationToken>());
    }
}