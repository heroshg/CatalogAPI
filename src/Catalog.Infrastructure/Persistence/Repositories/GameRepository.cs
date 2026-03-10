using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class GameRepository(CatalogDbContext context) : IGameRepository
{
    public async Task<Guid> AddAsync(Game game, CancellationToken ct)
    {
        await context.Games.AddAsync(game, ct);
        await context.SaveChangesAsync(ct);
        return game.Id;
    }

    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await context.Games.AsNoTracking().SingleOrDefaultAsync(g => g.Id == id, ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct) =>
        await context.Games.AnyAsync(g => g.Name.ToLower() == name.ToLower(), ct);

    public async Task<List<Game>> GetAllAsync(string name, int page, int pageSize, CancellationToken ct) =>
        await context.Games
            .AsNoTracking()
            .Where(g => g.IsActive && (name == "" || g.Name.ToLower().Contains(name.ToLower())))
            .OrderBy(g => g.CreatedAt)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
}
