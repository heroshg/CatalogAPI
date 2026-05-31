using Catalog.Application.Sagas;
using MassTransit;
using MassTransit.Saga;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Sagas;

/// <summary>
/// Repositório DynamoDB para <see cref="OrderSagaState"/>.
/// Usa <see cref="SagaRepository{TSaga}"/> do MassTransit como wrapper sobre a factory,
/// delegando todas as operações de ciclo de vida da saga para o contexto DynamoDB.
/// </summary>
public sealed class DynamoDbOrderSagaRepository : SagaRepository<OrderSagaState>
{
    public DynamoDbOrderSagaRepository(DynamoDbOrderSagaRepositoryContextFactory factory)
        : base(factory) { }
}
