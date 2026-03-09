using FiapCloudGames.Contracts.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using CatalogAPI.Application.Models;
using CatalogAPI.Domain.Interfaces;

namespace CatalogAPI.Application.Commands.PlaceOrder;

public class PlaceOrderHandler(
    IGameRepository gameRepository,
    IGameLicenseRepository licenseRepository,
    IPublishEndpoint publishEndpoint,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<PlaceOrderCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        var userEmail = httpContextAccessor.HttpContext?.User.FindFirst("username")?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return ResultViewModel<Guid>.Error("Invalid user token.");

        var game = await gameRepository.GetByIdAsync(request.GameId, ct);
        if (game is null)
            return ResultViewModel<Guid>.Error("Game not found.");

        if (await licenseRepository.ExistsAsync(game.Id, userId, ct))
            return ResultViewModel<Guid>.Error("You already own this game.");

        var orderId = Guid.NewGuid();

        await publishEndpoint.Publish(new OrderPlacedEvent(
            orderId,
            userId,
            userEmail ?? string.Empty,
            game.Id,
            game.Name,
            game.Price), ct);

        return ResultViewModel<Guid>.Success(orderId);
    }
}
