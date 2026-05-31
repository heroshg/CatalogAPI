using System.Text.Json;
using Catalog.Application.DTOs;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.Application.Queries.GetGamesByUser;

public class GetGamesByUserHandler(IGameLicenseRepository licenseRepository, IDistributedCache cache)
    : IRequestHandler<GetGamesByUserQuery, ResultViewModel<List<GameViewModel>>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public async Task<ResultViewModel<List<GameViewModel>>> Handle(GetGamesByUserQuery request, CancellationToken ct)
    {
        var cacheKey = CacheKeyFor(request.UserId);

        var cached = await cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
        {
            var hit = JsonSerializer.Deserialize<List<GameViewModel>>(cached)!;
            return ResultViewModel<List<GameViewModel>>.Success(hit);
        }

        var games = await licenseRepository.GetGamesByUserIdAsync(request.UserId, ct);
        var vms   = games.Select(g => new GameViewModel(
            g.Id, g.Name.Value, g.Description.Value, g.Price.Amount, g.IsActive, g.CreatedAt)).ToList();

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(vms),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl }, ct);

        return ResultViewModel<List<GameViewModel>>.Success(vms);
    }

    public static string CacheKeyFor(Guid userId) => $"games:lib:{userId}";
}
