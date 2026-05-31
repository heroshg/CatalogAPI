using Catalog.Domain.Interfaces;

namespace Catalog.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => Task.FromResult(0);
}
