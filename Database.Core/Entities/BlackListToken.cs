using Database.Core.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Database.Core.Entities;

[EntityTypeConfiguration(typeof(BlackListTokenConfiguration))]
public sealed class BlackListToken
{
    public required string Token { get; set; }
    public required DateTime ExpirationDate { get; set; }
}