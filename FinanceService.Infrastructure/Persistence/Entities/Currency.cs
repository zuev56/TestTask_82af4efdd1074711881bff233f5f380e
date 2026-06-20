using FinanceService.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Infrastructure.Persistence.Entities;

[EntityTypeConfiguration(typeof(CurrencyConfiguration))]
public sealed class Currency
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required decimal Rate { get; set; }
    public required bool IsDeleted { get; set; }
}