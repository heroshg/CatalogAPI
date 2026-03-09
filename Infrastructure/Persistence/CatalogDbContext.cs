using CatalogAPI.Domain.Entities;
using CatalogAPI.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Infrastructure.Persistence;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameLicense> GameLicenses => Set<GameLicense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GameConfiguration());
        modelBuilder.ApplyConfiguration(new GameLicenseConfiguration());
    }
}
