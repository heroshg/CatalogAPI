using Catalog.Application.Sagas;
using MassTransit;
using MassTransit.Saga;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Sagas;

public sealed class DynamoDbOrderSagaRepository : SagaRepository<OrderSagaState>
{
    public DynamoDbOrderSagaRepository(DynamoDbOrderSagaRepositoryContextFactory factory)
        : base(factory) { }
}
