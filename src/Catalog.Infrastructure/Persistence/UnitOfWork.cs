using Catalog.Domain.Interfaces;

namespace Catalog.Infrastructure.Persistence;

public class UnitOfWork(CatalogDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
}
