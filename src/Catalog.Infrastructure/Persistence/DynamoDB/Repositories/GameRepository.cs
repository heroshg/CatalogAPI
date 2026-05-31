using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Persistence.DynamoDB.Documents;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Repositories;

public class GameRepository(IDynamoDBContext db) : IGameRepository
{
    public async Task<Guid> AddAsync(Game game, CancellationToken ct)
    {
        await db.SaveAsync(GameDocument.FromDomain(game), ct);
        return game.Id;
    }

    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var doc = await db.LoadAsync<GameDocument>(id.ToString(), ct);
        return doc?.ToDomain();
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
    {
        var config = new DynamoDBOperationConfig { IndexName = "Name-Index" };
        var search = db.QueryAsync<GameDocument>(name.Trim(), config);
        var page   = await search.GetNextSetAsync(ct);
        return page.Count > 0;
    }

    public async Task<List<Game>> GetAllAsync(string name, int page, int pageSize, CancellationToken ct)
    {
        var config = new DynamoDBOperationConfig { IndexName = "ActiveGames-Index" };
        var search = db.QueryAsync<GameDocument>("ACTIVE", config);
        var all    = await search.GetRemainingAsync(ct);

        if (!string.IsNullOrWhiteSpace(name))
            all = all.Where(d => d.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

        return all
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(d => d.ToDomain())
            .ToList();
    }

    public async Task UpdateAsync(Game game, CancellationToken ct) =>
        await db.SaveAsync(GameDocument.FromDomain(game), ct);
}
