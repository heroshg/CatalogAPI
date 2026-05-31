using Amazon.DynamoDBv2.DataModel;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Persistence.DynamoDB.Documents;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Repositories;

public class GameLicenseRepository(IDynamoDBContext db) : IGameLicenseRepository
{
    public async Task AddAsync(GameLicense license, CancellationToken ct) =>
        await db.SaveAsync(GameLicenseDocument.FromDomain(license), ct);

    public async Task<bool> ExistsAsync(Guid gameId, Guid userId, CancellationToken ct)
    {
        // GetItem por PK (UserId) + SK (GameId) — O(1)
        var doc = await db.LoadAsync<GameLicenseDocument>(userId.ToString(), gameId.ToString(), ct);
        return doc is not null;
    }

    public async Task<List<Game>> GetGamesByUserIdAsync(Guid userId, CancellationToken ct)
    {
        // 1. Busca todas as licenças do usuário via Query no PK
        var search   = db.QueryAsync<GameLicenseDocument>(userId.ToString());
        var licenses = await search.GetRemainingAsync(ct);

        if (licenses.Count == 0)
            return [];

        // 2. BatchGet nos jogos referenciados pelas licenças
        var batch = db.CreateBatchGet<GameDocument>();
        foreach (var lic in licenses)
            batch.AddKey(lic.GameId);

        await batch.ExecuteAsync(ct);

        return batch.Results
            .Select(d => d.ToDomain())
            .ToList();
    }
}
