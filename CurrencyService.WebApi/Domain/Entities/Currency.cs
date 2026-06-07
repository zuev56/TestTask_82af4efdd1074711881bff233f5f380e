namespace CurrencyService.WebApi.Domain.Entities;

public sealed class Currency
{
    public required int Id { get; set; }
    public required string Name { get; set; } = null!;
    public required decimal Rate { get; set; }
}