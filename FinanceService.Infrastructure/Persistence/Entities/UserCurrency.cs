using FinanceService.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence.Entities;

[EntityTypeConfiguration(typeof(UserCurrencyConfiguration))]
public sealed class UserCurrency
{
    public required int UserId { get; set; }
    public required int CurrencyId { get; set; }
}