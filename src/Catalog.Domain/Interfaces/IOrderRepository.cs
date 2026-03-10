using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct);
    Task UpdateAsync(Order order, CancellationToken ct);
}
