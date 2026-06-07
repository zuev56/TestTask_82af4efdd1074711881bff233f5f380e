using CurrencyService.WebApi.Domain.Interfaces;
using Api.Common.Domain.Exceptions;
using CurrencyService.WebApi.Application.Features.Currency.Queries;
using CurrencyService.WebApi.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CurrencyService.Tests;

public sealed class GetUserCurrenciesQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetUserCurrenciesQuery_WhenUserHasCurrencies_ShouldReturnCurrencies()
    {
        // Arrange
        var userId = 11;
        var repository = Substitute.For<ICurrencyRepository>();
        var query = new GetUserCurrenciesQuery(userId);
        var handler = new GetUserCurrenciesQueryHandler(repository);
        var currencies = new []
        {
            new Currency { Id = 1, Name = "Юань", Rate = 10 },
            new Currency { Id = 2, Name = "Тенге", Rate = 20 }
        };
        repository.GetByUserIdAsync(userId, CancellationToken.None)
            .Returns(Task.FromResult<IReadOnlyList<Currency>>(currencies));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        await repository.Received(1).GetByUserIdAsync(userId, CancellationToken.None);
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Currencies.Should().BeEquivalentTo(currencies);
    }

    [Fact]
    public async Task Handle_GetUserCurrenciesQuery_WhenUserHasNotCurrencies_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = 11;
        var query = new GetUserCurrenciesQuery(userId);
        var repository = Substitute.For<ICurrencyRepository>();
        repository.GetByUserIdAsync(userId, CancellationToken.None)
            .Returns(Task.FromResult<IReadOnlyList<Currency>>([]));

        // Act
        var handler = new GetUserCurrenciesQueryHandler(repository);
        var act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}