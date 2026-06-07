using CurrencyService.WebApi.Domain.Interfaces;
using CurrencyService.WebApi.Application.Features.Currency.Queries;
using CurrencyService.WebApi.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace CurrencyService.Tests;

public sealed class GetAllCurrenciesQueryHandlerTests
{
    [Fact]
    public async Task Handle_GetAllCurrenciesQuery_WhenCalled_ShouldReturnAllCurrencies()
    {
        // Arrange
        var repository = Substitute.For<ICurrencyRepository>();
        var query = new GetAllCurrenciesQuery();
        var handler = new GetAllCurrenciesQueryHandler(repository);
        var currencies = new []
        {
            new Currency { Id = 1, Name = "Юань", Rate = 10 },
            new Currency { Id = 2, Name = "Тенге", Rate = 20 }
        };
        repository.GetAllAsync(CancellationToken.None)
            .Returns(Task.FromResult<IReadOnlyList<Currency>>(currencies));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        await repository.Received(1).GetAllAsync(CancellationToken.None);
        result.Should().NotBeNull();
        result.Currencies.Should().BeEquivalentTo(currencies);
    }
}