using Amazon.DynamoDBv2.DataModel;
using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Documents;

[DynamoDBTable("catalog-games")]
public class GameDocument
{
    // Chave primária da tabela
    [DynamoDBHashKey("Id")]
    public string Id { get; set; } = null!;

    // GSI1: Name-Index — hash key para ExistsByNameAsync
    [DynamoDBGlobalSecondaryIndexHashKey("Name-Index", AttributeName = "Name")]
    public string Name { get; set; } = null!;

    // GSI2: ActiveGames-Index (sparse) — presente só em jogos ativos
    // Quando IsActive=false, ActivePartition é nulo e o item sai do índice
    [DynamoDBGlobalSecondaryIndexHashKey("ActiveGames-Index", AttributeName = "ActivePartition")]
    public string? ActivePartition { get; set; }

    // Range key do GSI2 para ordenação por data de criação
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
