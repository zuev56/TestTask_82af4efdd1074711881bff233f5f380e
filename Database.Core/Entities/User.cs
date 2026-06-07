using Database.Core.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Database.Core.Entities;

[EntityTypeConfiguration(typeof(UserConfiguration))]
public sealed class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string PasswordHash { get; set; }

    public ICollection<Currency> Currencies { get; set; } = [];
}