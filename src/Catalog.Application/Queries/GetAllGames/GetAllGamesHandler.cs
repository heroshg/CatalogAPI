using System.Text.Json;
using Catalog.Application.DTOs;
using Catalog.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.Application.Queries.GetAllGames;

public class GetAllGamesHandler(IGameRepository repository, IDistributedCache cache)
    : IRequestHandler<GetAllGamesQuery, ResultViewModel<PageResultViewModel<GameViewModel>>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public async Task<ResultViewModel<PageResultViewModel<GameViewModel>>> Handle(GetAllGamesQuery request, CancellationToken ct)
    {
        var cacheKey = $"games:list:name={request.Name}:page={request.Page}:size={request.PageSize}";

        var cached = await cache.GetStringAsync(cacheKey, ct);
        if (cached is not null)
        {
            var cachedResult = JsonSerializer.Deserialize<PageResultViewModel<GameViewModel>>(cached)!;
            return ResultViewModel<PageResultViewModel<GameViewModel>>.Success(cachedResult);
        }

        var games = await repository.GetAllAsync(request.Name, request.Page, request.PageSize, ct);
        var vms   = games.Select(g => new GameViewModel(g.Id, g.Name.Value, g.Description.Value, g.Price.Amount, g.IsActive, g.CreatedAt)).ToList();
        var result = new PageResultViewModel<GameViewModel>(vms, vms.Count, request.Page, request.PageSize);

        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = CacheTtl }, ct);

        return ResultViewModel<PageResultViewModel<GameViewModel>>.Success(result);
    }
}
