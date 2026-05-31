using Amazon.DynamoDBv2.DataModel;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Persistence.DynamoDB.Documents;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Repositories;

public class OrderRepository(IDynamoDBContext db) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken ct) =>
        await db.SaveAsync(OrderDocument.FromDomain(order), ct);

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct)
    {
        var doc = await db.LoadAsync<OrderDocument>(orderId.ToString(), ct);
        return doc?.ToDomain();
    }

    public async Task UpdateAsync(Order order, CancellationToken ct) =>
        await db.SaveAsync(OrderDocument.FromDomain(order), ct);
}
