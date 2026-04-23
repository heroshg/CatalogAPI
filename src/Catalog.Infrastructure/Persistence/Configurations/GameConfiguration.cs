using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasConversion(v => v.Value, v => GameName.From(v))
            .IsRequired()
            .HasMaxLength(GameName.MaxLength);

        builder.Property(g => g.Description)
            .HasConversion(v => v.Value, v => GameDescription.From(v))
            .IsRequired()
            .HasMaxLength(GameDescription.MaxLength);

        builder.Property(g => g.Price)
            .HasConversion(v => v.Amount, v => Money.Of(v))
            .IsRequired()
            .HasPrecision(10, 2);

        builder.HasIndex(g => g.Name).IsUnique();
    }
}
