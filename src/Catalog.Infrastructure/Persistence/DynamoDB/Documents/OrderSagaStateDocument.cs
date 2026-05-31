using Amazon.DynamoDBv2.DataModel;
using Catalog.Application.Sagas;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Documents;

[DynamoDBTable("catalog-saga-states")]
public class OrderSagaStateDocument
{
    [DynamoDBHashKey("CorrelationId")]
    public string CorrelationId { get; set; } = null!;

    public string CurrentState { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string GameId { get; set; } = null!;
    public string GameName { get; set; } = null!;
    public decimal Price { get; set; }
    public string CreatedAt { get; set; } = null!;

    public static OrderSagaStateDocument FromState(OrderSagaState state) => new()
    {
        CorrelationId = state.CorrelationId.ToString(),
        CurrentState  = state.CurrentState,
        UserId        = state.UserId.ToString(),
        UserEmail     = state.UserEmail,
        GameId        = state.GameId.ToString(),
        GameName      = state.GameName,
        Price         = state.Price,
        CreatedAt     = state.CreatedAt.ToString("O"),
    };

    public OrderSagaState ToState() => new()
    {
        CorrelationId = Guid.Parse(CorrelationId),
        CurrentState  = CurrentState,
        UserId        = Guid.Parse(UserId),
        UserEmail     = UserEmail,
        GameId        = Guid.Parse(GameId),
        GameName      = GameName,
        Price         = Price,
        CreatedAt     = DateTime.Parse(CreatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind),
    };
}
