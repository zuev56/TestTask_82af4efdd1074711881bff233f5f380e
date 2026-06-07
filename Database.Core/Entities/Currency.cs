using Database.Core.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Database.Core.Entities;

[EntityTypeConfiguration(typeof(CurrencyConfiguration))]
public sealed class Currency
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required decimal Rate { get; set; }
}