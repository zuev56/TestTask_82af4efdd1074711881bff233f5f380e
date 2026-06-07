namespace Api.Common.Domain.Entities;

public sealed class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}