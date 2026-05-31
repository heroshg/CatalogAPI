using Amazon.DynamoDBv2.DataModel;
using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Documents;

[DynamoDBTable("catalog-games")]
public class GameDocument
{
    [DynamoDBHashKey("Id")]
    public string Id { get; set; } = null!;

    [DynamoDBGlobalSecondaryIndexHashKey("Name-Index", AttributeName = "Name")]
    public string Name { get; set; } = null!;

    [DynamoDBGlobalSecondaryIndexHashKey("ActiveGames-Index", AttributeName = "ActivePartition")]
    public string? ActivePartition { get; set; }

    [DynamoDBGlobalSecondaryIndexRangeKey("ActiveGames-Index", AttributeName = "CreatedAt")]
    public string CreatedAt { get; set; } = null!;

    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public string UpdatedAt { get; set; } = null!;

    private const string ActiveKey = "ACTIVE";

    public static GameDocument FromDomain(Game game) => new()
    {
        Id            = game.Id.ToString(),
        Name          = game.Name.Value,
        Description   = game.Description.Value,
        Price         = game.Price.Amount,
        IsActive      = game.IsActive,
        ActivePartition = game.IsActive ? ActiveKey : null,
        CreatedAt     = game.CreatedAt.ToString("O"),
        UpdatedAt     = game.UpdatedAt.ToString("O"),
    };

    public Game ToDomain() =>
        Game.Reconstitute(
            Guid.Parse(Id),
            Name,
            Description,
            Price,
            IsActive,
            DateTime.Parse(CreatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind),
            DateTime.Parse(UpdatedAt, null, System.Globalization.DateTimeStyles.RoundtripKind));
}
