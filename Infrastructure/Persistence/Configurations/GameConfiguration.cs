using CatalogAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogAPI.Infrastructure.Persistence.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).IsRequired().HasMaxLength(80);
        builder.Property(g => g.Description).IsRequired().HasMaxLength(300);
        builder.Property(g => g.Price).IsRequired().HasPrecision(10, 2);
        builder.HasIndex(g => g.Name).IsUnique();
    }
}
