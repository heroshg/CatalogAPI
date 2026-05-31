using Amazon.DynamoDBv2.DataModel;
using Catalog.Domain.Entities;
using GameNameVO = Catalog.Domain.ValueObjects.GameName;
using MoneyVO    = Catalog.Domain.ValueObjects.Money;
using OrderStatusVO = Catalog.Domain.ValueObjects.OrderStatus;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Documents;

[DynamoDBTable("catalog-orders")]
public class OrderDocument
{
    [DynamoDBHashKey("Id")]
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public string GameId { get; set; } = null!;
    public string GameName { get; set; } = null!;
    public decimal Price { get; set; }
    public string Status { get; set; } = null!;
    public string CreatedAt { get; set; } = null!;
    public string? UpdatedAt { get; set; }

    public static OrderDocument FromDomain(Order order) => new()
    {
        Id        = order.Id.ToString(),
        UserId    = order.UserId.ToString(),
        GameId    = order.GameId.ToString(),
        GameName  = order.GameName.Value,
        Price     = order.Price.Amount,
        Status    = order.Status.Value,
        CreatedAt = order.CreatedAt.ToString("O"),
        UpdatedAt = order.UpdatedAt?.ToString("O"),
    };

    public Order ToDomain() =>
        Order.Reconstitute(
            Guid.Parse(Id),
            Guid.Parse(UserId),
            Guid.Parse(GameId),
            GameNameVO.From(GameName),
            MoneyVO.Of(Price),
            Status switch
            {
                "Completed" => OrderStatusVO.Completed,
                "Failed"    => OrderStatusVO.Failed,
                _           => OrderStatusVO.Pending
            },
            DateTime.Parse(CreatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind),
            UpdatedAt is null
                ? null
                : DateTime.Parse(UpdatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind));
}
