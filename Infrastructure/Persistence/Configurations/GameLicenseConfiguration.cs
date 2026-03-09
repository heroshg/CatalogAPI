using CatalogAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogAPI.Infrastructure.Persistence.Configurations;

public class GameLicenseConfiguration : IEntityTypeConfiguration<GameLicense>
{
    public void Configure(EntityTypeBuilder<GameLicense> builder)
    {
        builder.HasKey(gl => gl.Id);
        builder.Property(gl => gl.GameId).IsRequired();
        builder.Property(gl => gl.UserId).IsRequired();
        builder.HasIndex(gl => new { gl.GameId, gl.UserId }).IsUnique();
    }
}
