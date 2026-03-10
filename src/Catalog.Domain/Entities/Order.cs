namespace Catalog.Domain.Entities;

public class Order
{
    private Order() { }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public string GameName { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public string Status { get; private set; } = string.Empty; // Pending | Completed | Failed
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Order Create(Guid orderId, Guid userId, Guid gameId, string gameName, decimal price)
    {
        return new Order
        {
            Id = orderId,
            UserId = userId,
            GameId = gameId,
            GameName = gameName,
            Price = price,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Complete()
    {
        Status = "Completed";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = "Failed";
        UpdatedAt = DateTime.UtcNow;
    }
}
