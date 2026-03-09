using CatalogAPI.Domain.Entities;

namespace CatalogAPI.Domain.Interfaces;

public interface IGameLicenseRepository
{
    Task AddAsync(GameLicense license, CancellationToken ct);
    Task<bool> ExistsAsync(Guid gameId, Guid userId, CancellationToken ct);
    Task<List<Game>> GetGamesByUserIdAsync(Guid userId, CancellationToken ct);
}
