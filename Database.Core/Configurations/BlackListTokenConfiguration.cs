using Database.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Core.Configurations;

public sealed class BlackListTokenConfiguration : IEntityTypeConfiguration<BlackListToken>
{
    public void Configure(EntityTypeBuilder<BlackListToken> builder)
    {
        builder.ToTable("jwt_token_blacklist");

        builder.HasKey(x => x.Token);

        builder.Property(x => x.ExpirationDate)
            .HasColumnName("expiration_date")
            .IsRequired();
    }
}