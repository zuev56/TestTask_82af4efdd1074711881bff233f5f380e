using FinanceService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceService.Infrastructure.Persistence.Configurations;

public sealed class UserCurrencyConfiguration : IEntityTypeConfiguration<UserCurrency>
{
    public void Configure(EntityTypeBuilder<UserCurrency> builder)
    {
        builder.ToTable("user_currencies", "finance");

        builder.HasKey(x => new { x.UserId, x.CurrencyId });

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.CurrencyId)
            .HasColumnName("currency_id")
            .IsRequired();

        builder.HasOne<Currency>()
            .WithMany()
            .HasForeignKey(x => x.CurrencyId);
    }
}