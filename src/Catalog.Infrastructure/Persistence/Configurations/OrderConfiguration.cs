using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.GameName).HasMaxLength(512).IsRequired();
        builder.Property(o => o.Status).HasMaxLength(64).IsRequired();
        builder.Property(o => o.Price).HasColumnType("decimal(18,2)");
        builder.HasIndex(o => o.UserId);
    }
}
