using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Persistence.Configurations;

namespace UserService.Infrastructure.Persistence.Entities;

[EntityTypeConfiguration(typeof(UserConfiguration))]
public sealed class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string PasswordHash { get; set; }
}