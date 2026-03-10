using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces;

public interface IGameRepository
{
    Task<Guid> AddAsync(Game game, CancellationToken ct);
    Task<Game?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
    Task<List<Game>> GetAllAsync(string name, int page, int pageSize, CancellationToken ct);
}
