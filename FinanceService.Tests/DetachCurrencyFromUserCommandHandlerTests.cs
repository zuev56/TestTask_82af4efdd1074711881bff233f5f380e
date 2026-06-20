using FinanceService.Application.Features.Currency.Commands;
using FinanceService.Application.Interfaces;
using NSubstitute;
using Xunit;

namespace FinanceService.Tests;

public sealed class DetachCurrencyFromUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_DetachCurrencyFromUserCommand_WhenCalledWithValidData_ShouldDetachCurrency()
    {
        // Arrange
        int currencyId = 1, userId = 2;
        var repository = Substitute.For<ICurrencyRepository>();
        var command = new DetachCurrencyFromUserCommand(currencyId, userId);
        var handler = new DetachCurrencyFromUserCommandHandler(repository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await repository.Received(1).DetachFromUserAsync(currencyId, userId, CancellationToken.None);
    }
}