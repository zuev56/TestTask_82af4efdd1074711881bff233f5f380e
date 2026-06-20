using FluentAssertions;
using NSubstitute;
using Shared.Domain.Entities;
using Shared.Domain.Exceptions;
using UserService.Application.Features.Users.Queries;
using UserService.Application.Interfaces;
using Xunit;

namespace UserService.Tests.Users;

public sealed class GetUserQueryHandlerTests
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
    }

    [Fact]
    public async Task Handle_GetUserQuery_WhenUserNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = 11;
        var query = new GetUserQuery(userId);
        var handler = new GetUserQueryHandler(_userRepository);

        _userRepository.GetByIdAsync(userId, CancellationToken.None)
            .Returns(Task.FromResult<User?>(null));

        // Act
        var act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_GetUserQuery_WhenUserFound_ShouldReturnUserResponse()
    {
        // Arrange
        var userId = 11;
        var userName = "user1";
        var query = new GetUserQuery(userId);
        var handler = new GetUserQueryHandler(_userRepository);

        _userRepository.GetByIdAsync(userId, CancellationToken.None)
            .Returns(Task.FromResult<User?>(new User { Id = userId, Name = userName }));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Name.Should().Be(userName);
    }
}