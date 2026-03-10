using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.Infrastructure.Persistence.Repositories;

public class OrderRepository(CatalogDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken ct)
    {
        await dbContext.Orders.AddAsync(order, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct)
        => await dbContext.Orders.FindAsync(new object[] { orderId }, ct);

    public async Task UpdateAsync(Order order, CancellationToken ct)
    {
        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(ct);
    }
}
