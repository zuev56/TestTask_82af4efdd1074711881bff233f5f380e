using Database.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Core.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasColumnName("password")
            .IsRequired();

        builder.HasMany(u => u.Currencies)
            .WithMany()
            .UsingEntity<Dictionary<int, int>>("favorites",
                j => j.HasOne<Currency>().WithMany().HasForeignKey("currency_id"),
                j => j.HasOne<User>().WithMany().HasForeignKey("user_id"),
                j => j.HasKey("user_id", "currency_id"));
    }
}