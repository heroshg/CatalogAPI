using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class GameLicenseRepository(CatalogDbContext context) : IGameLicenseRepository
{
    public async Task AddAsync(GameLicense license, CancellationToken ct)
    {
        await context.GameLicenses.AddAsync(license, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid gameId, Guid userId, CancellationToken ct) =>
        await context.GameLicenses.AnyAsync(gl => gl.GameId == gameId && gl.UserId == userId, ct);

    public async Task<List<Game>> GetGamesByUserIdAsync(Guid userId, CancellationToken ct) =>
        await (from gl in context.GameLicenses.AsNoTracking()
               join g in context.Games.AsNoTracking() on gl.GameId equals g.Id
               where gl.UserId == userId
               select g).ToListAsync(ct);
}
