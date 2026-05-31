using System.Text.Json;
using MediatR;
using Catalog.Application.DTOs;
using Catalog.Application.Queries.GetAllGames;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.Application.Commands.RegisterGame;

public class RegisterGameHandler(IGameRepository repository, IDistributedCache cache)
    : IRequestHandler<RegisterGameCommand, ResultViewModel<Guid>>
{
    private static readonly TimeSpan GameCacheTtl = TimeSpan.FromMinutes(10);

    public async Task<ResultViewModel<Guid>> Handle(RegisterGameCommand request, CancellationToken ct)
    {
        if (await repository.ExistsByNameAsync(request.Name, ct))
            return ResultViewModel<Guid>.Error("A game with this name already exists.");

        var game = new Game(request.Name, request.Description, request.Price);
        var id   = await repository.AddAsync(game, ct);

        var vm = new GameViewModel(game.Id, game.Name.Value, game.Description.Value, game.Price.Amount, game.IsActive, game.CreatedAt);
        await cache.SetStringAsync(
            GameCacheKey(id),
            JsonSerializer.Serialize(vm),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = GameCacheTtl }, ct);

        await cache.RemoveAsync(GetAllGamesHandler.RevisionKey, ct);

        return ResultViewModel<Guid>.Success(id);
    }

    public static string GameCacheKey(Guid id) => $"game:{id}";
}
