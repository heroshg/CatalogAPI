using Catalog.Application.Commands.PlaceOrder;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Domain.ValueObjects;
using FiapCloudGames.Contracts.Events;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Catalog.Tests.Application;

public class PlaceOrderHandlerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly PlaceOrderCommand Cmd = new(GameId);

    private static IHttpContextAccessor BuildAccessorWithUser(Guid userId, string email = "player@example.com")
    {
        var claims = new[]
        {
            new Claim("userId", userId.ToString()),
            new Claim("username", email)
        };
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims)) };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(context);
        return accessor.Object;
    }

    private static IHttpContextAccessor BuildAccessorWithoutClaims()
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(new DefaultHttpContext());
        return accessor.Object;
    }

    private static Game CreateGame() => new("Hades", "A roguelike.", 59.90m);

    [Fact]
    public async Task MissingToken_Handle_ReturnsError()
    {
        var sut = new PlaceOrderHandler(
            new Mock<IGameRepository>().Object,
            new Mock<IGameLicenseRepository>().Object,
            new Mock<IOrderRepository>().Object,
            new Mock<IUnitOfWork>().Object,
            new Mock<IPublishEndpoint>().Object,
            BuildAccessorWithoutClaims());

        var result = await sut.Handle(Cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid user token.", result.Message);
    }

    [Fact]
    public async Task GameNotFound_Handle_ReturnsError()
    {
        var gameRepo = new Mock<IGameRepository>();
        gameRepo.Setup(r => r.GetByIdAsync(GameId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Game?)null);

        var sut = new PlaceOrderHandler(
            gameRepo.Object,
            new Mock<IGameLicenseRepository>().Object,
            new Mock<IOrderRepository>().Object,
            new Mock<IUnitOfWork>().Object,
            new Mock<IPublishEndpoint>().Object,
            BuildAccessorWithUser(UserId));

        var result = await sut.Handle(Cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Game not found.", result.Message);
    }

    [Fact]
    public async Task UserAlreadyOwnsGame_Handle_ReturnsError()
    {
        var game = CreateGame();

        var gameRepo = new Mock<IGameRepository>();
        gameRepo.Setup(r => r.GetByIdAsync(GameId, It.IsAny<CancellationToken>())).ReturnsAsync(game);

        var licenseRepo = new Mock<IGameLicenseRepository>();
        licenseRepo.Setup(r => r.ExistsAsync(game.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new PlaceOrderHandler(
            gameRepo.Object,
            licenseRepo.Object,
            new Mock<IOrderRepository>().Object,
            new Mock<IUnitOfWork>().Object,
            new Mock<IPublishEndpoint>().Object,
            BuildAccessorWithUser(UserId));

        var result = await sut.Handle(Cmd, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("You already own this game.", result.Message);
    }

    [Fact]
    public async Task Valid_Handle_CreatesOrderPublishesEventReturnsOrderId()
    {
        var game = CreateGame();

        var gameRepo = new Mock<IGameRepository>();
        gameRepo.Setup(r => r.GetByIdAsync(GameId, It.IsAny<CancellationToken>())).ReturnsAsync(game);

        var licenseRepo = new Mock<IGameLicenseRepository>();
        licenseRepo.Setup(r => r.ExistsAsync(game.Id, UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var orderRepo = new Mock<IOrderRepository>();
        var uow       = new Mock<IUnitOfWork>();
        var publisher = new Mock<IPublishEndpoint>();

        var sut = new PlaceOrderHandler(
            gameRepo.Object,
            licenseRepo.Object,
            orderRepo.Object,
            uow.Object,
            publisher.Object,
            BuildAccessorWithUser(UserId, "player@example.com"));

        var result = await sut.Handle(Cmd, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);

        orderRepo.Verify(r => r.AddAsync(
            It.Is<Order>(o =>
                o.UserId == UserId &&
                o.GameId == game.Id &&
                o.Status == OrderStatus.Pending),
            It.IsAny<CancellationToken>()), Times.Once);

        publisher.Verify(p => p.Publish(
            It.Is<OrderPlacedEvent>(e =>
                e.UserId == UserId &&
                e.GameId == game.Id &&
                e.UserEmail == "player@example.com"),
            It.IsAny<CancellationToken>()), Times.Once);

        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
