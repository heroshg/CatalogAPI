using Catalog.Domain.Exceptions;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities;

public class Order
{
    private Order() { }

    public Guid        Id        { get; private set; }
    public Guid        UserId    { get; private set; }
    public Guid        GameId    { get; private set; }
    public GameName    GameName  { get; private set; } = null!;
    public Money       Price     { get; private set; } = null!;
    public OrderStatus Status    { get; private set; } = null!;
    public DateTime    CreatedAt { get; private set; }
    public DateTime?   UpdatedAt { get; private set; }

    /// <summary>O agregado gera sua própria identidade.</summary>
    public static Order Create(Guid userId, Guid gameId, GameName gameName, Money price)
    {
        return new Order
        {
            Id        = Guid.NewGuid(),
            UserId    = userId,
            GameId    = gameId,
            GameName  = gameName,
            Price     = price,
            Status    = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Complete()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot complete an order in status '{Status.Value}'.");
        Status    = OrderStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot fail an order in status '{Status.Value}'.");
        Status    = OrderStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}
