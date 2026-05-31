using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Persistence.DynamoDB;

public sealed class DynamoDbBootstrapper(IAmazonDynamoDB client, ILogger<DynamoDbBootstrapper> logger)
{
    public Task EnsureTablesExistAsync(CancellationToken ct = default) =>
        // Todas as 4 tabelas são criadas em paralelo — startup ~4x mais rápido
        Task.WhenAll(
            EnsureGamesTableAsync(ct),
            EnsureLicensesTableAsync(ct),
            EnsureOrdersTableAsync(ct),
            EnsureSagaStatesTableAsync(ct));

    private Task EnsureGamesTableAsync(CancellationToken ct) =>
        CreateIfNotExistsAsync(new CreateTableRequest
        {
            TableName            = "catalog-games",
            BillingMode          = BillingMode.PAY_PER_REQUEST,
            KeySchema            = [new("Id", KeyType.HASH)],
            AttributeDefinitions =
            [
                new("Id",              ScalarAttributeType.S),
                new("Name",            ScalarAttributeType.S),
                new("ActivePartition", ScalarAttributeType.S),
                new("CreatedAt",       ScalarAttributeType.S),
            ],
            GlobalSecondaryIndexes =
            [
                new()
                {
                    IndexName  = "Name-Index",
                    KeySchema  = [new("Name", KeyType.HASH)],
                    Projection = new Projection { ProjectionType = ProjectionType.KEYS_ONLY },
                },
                new()
                {
                    IndexName  = "ActiveGames-Index",
                    KeySchema  =
                    [
                        new("ActivePartition", KeyType.HASH),
                        new("CreatedAt",       KeyType.RANGE),
                    ],
                    Projection = new Projection { ProjectionType = ProjectionType.ALL },
                },
            ],
        }, ct);

    private Task EnsureLicensesTableAsync(CancellationToken ct) =>
        CreateIfNotExistsAsync(new CreateTableRequest
        {
            TableName            = "catalog-licenses",
            BillingMode          = BillingMode.PAY_PER_REQUEST,
            KeySchema            = [new("UserId", KeyType.HASH), new("GameId", KeyType.RANGE)],
            AttributeDefinitions =
            [
                new("UserId", ScalarAttributeType.S),
                new("GameId", ScalarAttributeType.S),
            ],
        }, ct);

    private Task EnsureOrdersTableAsync(CancellationToken ct) =>
        CreateIfNotExistsAsync(new CreateTableRequest
        {
            TableName            = "catalog-orders",
            BillingMode          = BillingMode.PAY_PER_REQUEST,
            KeySchema            = [new("Id", KeyType.HASH)],
            AttributeDefinitions = [new("Id", ScalarAttributeType.S)],
        }, ct);

    private Task EnsureSagaStatesTableAsync(CancellationToken ct) =>
        CreateIfNotExistsAsync(new CreateTableRequest
        {
            TableName            = "catalog-saga-states",
            BillingMode          = BillingMode.PAY_PER_REQUEST,
            KeySchema            = [new("CorrelationId", KeyType.HASH)],
            AttributeDefinitions = [new("CorrelationId", ScalarAttributeType.S)],
        }, ct);

    private async Task CreateIfNotExistsAsync(CreateTableRequest request, CancellationToken ct)
    {
        try
        {
            await client.DescribeTableAsync(request.TableName, ct);
            logger.LogDebug("DynamoDB table '{Table}' already exists.", request.TableName);
            return;
        }
        catch (ResourceNotFoundException) { }

        logger.LogInformation("Creating DynamoDB table '{Table}'...", request.TableName);
        await client.CreateTableAsync(request, ct);
        await WaitUntilActiveAsync(request.TableName, ct);
        logger.LogInformation("DynamoDB table '{Table}' is ready.", request.TableName);
    }

    private async Task WaitUntilActiveAsync(string tableName, CancellationToken ct)
    {
        // Checa imediatamente primeiro (LocalStack ativa tabelas em <100ms),
        // depois aguarda com backoff até 30s
        for (var i = 0; i < 30; i++)
        {
            var desc = await client.DescribeTableAsync(tableName, ct);
            if (desc.Table.TableStatus == TableStatus.ACTIVE) return;
            await Task.Delay(TimeSpan.FromSeconds(1), ct);
        }
    }
}
