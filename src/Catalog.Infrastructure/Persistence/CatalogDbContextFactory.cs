using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catalog.Infrastructure.Persistence;

// Usado apenas pelo `dotnet ef` em design-time. Em runtime o DbContext é
// resolvido via DI (AddDbContext) com a connection string da configuração.
public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=catalog_db;Username=postgres;Password=design-time")
            .Options;
        return new CatalogDbContext(opts);
    }
}
