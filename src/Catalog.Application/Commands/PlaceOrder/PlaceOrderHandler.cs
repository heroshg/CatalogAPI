using System.Text.Json;
using FiapCloudGames.Contracts.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Catalog.Application.Commands.RegisterGame;
using Catalog.Application.DTOs;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.Application.Commands.PlaceOrder;

public class PlaceOrderHandler(
    IGameRepository gameRepository,
    IGameLicenseRepository licenseRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    IHttpContextAccessor httpContextAccessor,
    IDistributedCache cache)
    : IRequestHandler<PlaceOrderCommand, ResultViewModel<Guid>>
{
    private static readonly TimeSpan GameCacheTtl = TimeSpan.FromMinutes(10);

    public async Task<ResultViewModel<Guid>> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        var userEmail   = httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return ResultViewModel<Guid>.Error("Invalid user token.");

        var game = await GetGameCachedAsync(request.GameId, ct);
        if (game is null)
            return ResultViewModel<Guid>.Error("Game not found.");

        if (!game.IsActive)
            return ResultViewModel<Guid>.Error("Game is not available.");

        if (await licenseRepository.ExistsAsync(game.Id, userId, ct))
            return ResultViewModel<Guid>.Error("You already own this game.");

        var order = Order.Create(userId, game.Id, game.Name, game.Price);
        await orderRepository.AddAsync(order, ct);

        await publishEndpoint.Publish(new OrderPlacedEvent(
            order.Id, userId, userEmail ?? string.Empty,
            game.Id, game.Name.Value, game.Price.Amount), ct);

        await unitOfWork.SaveChangesAsync(ct);

        return ResultViewModel<Guid>.Success(order.Id);
    }

    private async Task<Game?> GetGameCachedAsync(Guid gameId, CancellationToken ct)
    {
        var cacheKey = RegisterGameHandler.GameCacheKey(gameId);
        var cached   = await cache.GetStringAsync(cacheKey, ct);

        if (cached is not null)
        {
            var vm = JsonSerializer.Deserialize<GameViewModel>(cached)!;
            return Game.Reconstitute(vm.Id, vm.Name, vm.Description, vm.Price, vm.IsActive, vm.CreatedAt, vm.CreatedAt);
        }

        var game = await gameRepository.GetByIdAsync(gameId, ct);
        if (game is null) return null;

        var gameVm = new GameViewModel(game.Id, game.Name.Value, game.Description.Value, game.Price.Amount, game.IsActive, game.CreatedAt);
        await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(gameVm),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = GameCacheTtl }, ct);

        return game;
    }
}
