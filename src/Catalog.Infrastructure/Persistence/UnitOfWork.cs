using Catalog.Domain.Interfaces;

namespace Catalog.Infrastructure.Persistence;

// Com DynamoDB cada SaveAsync do repositório é imediato.
// IUnitOfWork permanece na interface pública para não quebrar os handlers.
public class UnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => Task.FromResult(0);
}
