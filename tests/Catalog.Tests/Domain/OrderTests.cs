using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;
using Catalog.Domain.ValueObjects;

namespace Catalog.Tests.Domain;

public class OrderTests
{
    private static Order CreateOrder() =>
        Order.Create(Guid.NewGuid(), Guid.NewGuid(), GameName.From("Hades"), Money.Of(59.90m));

    [Fact]
    public void Create_SetsAllPropertiesWithPendingStatus()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var name   = GameName.From("Hades");
        var price  = Money.Of(59.90m);

        var order = Order.Create(userId, gameId, name, price);

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(userId, order.UserId);
        Assert.Equal(gameId, order.GameId);
        Assert.Equal("Hades", order.GameName.Value);
        Assert.Equal(59.90m, order.Price.Amount);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Null(order.UpdatedAt);
    }

    [Fact]
    public void Complete_PendingOrder_SetsStatusCompletedAndUpdatedAt()
    {
        var order = CreateOrder();
        var before = DateTime.UtcNow;

        order.Complete();

        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.NotNull(order.UpdatedAt);
        Assert.True(order.UpdatedAt >= before);
    }

    [Fact]
    public void Fail_PendingOrder_SetsStatusFailedAndUpdatedAt()
    {
        var order = CreateOrder();
        var before = DateTime.UtcNow;

        order.Fail();

        Assert.Equal(OrderStatus.Failed, order.Status);
        Assert.NotNull(order.UpdatedAt);
        Assert.True(order.UpdatedAt >= before);
    }

    [Fact]
    public void Complete_AlreadyCompletedOrder_ThrowsDomainException()
    {
        var order = CreateOrder();
        order.Complete();

        Assert.Throws<DomainException>(() => order.Complete());
    }

    [Fact]
    public void Fail_AlreadyCompletedOrder_ThrowsDomainException()
    {
        var order = CreateOrder();
        order.Complete();

        Assert.Throws<DomainException>(() => order.Fail());
    }

    [Fact]
    public void Complete_AlreadyFailedOrder_ThrowsDomainException()
    {
        var order = CreateOrder();
        order.Fail();

        Assert.Throws<DomainException>(() => order.Complete());
    }
}
