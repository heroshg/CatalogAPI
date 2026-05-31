using Amazon.DynamoDBv2.DataModel;
using Catalog.Application.Sagas;
using MassTransit;
using MassTransit.Saga;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Sagas;

/// <summary>
/// Factory que cria o contexto de repositório DynamoDB para cada mensagem processada pela saga.
/// Implementa <see cref="ISagaRepositoryContextFactory{TSaga}"/>, o ponto de extensão
/// oficial do MassTransit 8 para repositórios customizados.
/// </summary>
public sealed class DynamoDbOrderSagaRepositoryContextFactory(IDynamoDBContext db)
    : ISagaRepositoryContextFactory<OrderSagaState>
{
    public void Probe(ProbeContext context) =>
        context.CreateFilterScope("dynamodb-order-saga-repository");

    public Task Send<T>(
        ConsumeContext<T> context,
        IPipe<SagaRepositoryContext<OrderSagaState, T>> next)
        where T : class =>
        next.Send(new DynamoDbOrderSagaRepositoryContext<T>(context, db));

    public Task SendQuery<T>(
        ConsumeContext<T> context,
        ISagaQuery<OrderSagaState> query,
        IPipe<SagaRepositoryQueryContext<OrderSagaState, T>> next)
        where T : class =>
        throw new NotSupportedException(
            "DynamoDbOrderSagaRepository does not support query-based correlation.");
}
