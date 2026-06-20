using FinanceService.Application.Features.Currency.Commands;
using FinanceService.Application.Interfaces;
using NSubstitute;
using Xunit;

namespace FinanceService.Tests;

public sealed class AttachCurrencyToUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_AttachCurrencyToUserCommand_WhenCalledWithValidData_ShouldAttachCurrency()
    {
        // Arrange
        int currencyId = 1, userId = 2;
        var repository = Substitute.For<ICurrencyRepository>();
        var command = new AttachCurrencyToUserCommand(currencyId, userId);
        var handler = new AttachCurrencyToUserCommandHandler(repository);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        await repository.Received(1).AttachToUserAsync(currencyId, userId, CancellationToken.None);
    }
}