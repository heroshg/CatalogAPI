using Amazon.DynamoDBv2.DataModel;
using Catalog.Domain.Entities;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Documents;

[DynamoDBTable("catalog-licenses")]
public class GameLicenseDocument
{
    // PK: UserId — permite QueryAsync(userId) para buscar licenças do usuário
    [DynamoDBHashKey("UserId")]
    public string UserId { get; set; } = null!;

    // SK: GameId — junto com UserId forma a chave composta (evita duplicatas via GetItem)
    [DynamoDBRangeKey("GameId")]
    public string GameId { get; set; } = null!;

    public string Id { get; set; } = null!;
    public string AcquiredAt { get; set; } = null!;

    public static GameLicenseDocument FromDomain(GameLicense license) => new()
    {
        Id         = license.Id.ToString(),
        UserId     = license.UserId.ToString(),
        GameId     = license.GameId.ToString(),
        AcquiredAt = license.AcquiredAt.ToString("O"),
    };

    public GameLicense ToDomain() =>
        GameLicense.Reconstitute(
            Guid.Parse(Id),
            Guid.Parse(GameId),
            Guid.Parse(UserId),
            DateTime.Parse(AcquiredAt, null, System.Globalization.DateTimeStyles.RoundtripKind));
}
