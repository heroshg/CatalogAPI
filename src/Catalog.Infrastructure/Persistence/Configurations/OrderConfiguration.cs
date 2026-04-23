using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.GameName)
            .HasConversion(v => v.Value, v => GameName.From(v))
            .HasMaxLength(GameName.MaxLength)
            .IsRequired();

        builder.Property(o => o.Price)
            .HasConversion(v => v.Amount, v => Money.Of(v))
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion(v => v.Value, v => v == "Pending"    ? OrderStatus.Pending
                                             : v == "Completed"  ? OrderStatus.Completed
                                             :                     OrderStatus.Failed)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(o => o.UserId);
    }
}
