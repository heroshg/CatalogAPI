using Catalog.Application.Queries.GetGamesByUser;
using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using FiapCloudGames.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.Application.Sagas.Activities;

public class ApproveOrderActivity(
    IGameLicenseRepository licenseRepository,
    IOrderRepository orderRepository,
    IDistributedCache cache)
    : IStateMachineActivity<OrderSagaState, PaymentProcessedEvent>
{
    public async Task Execute(
        BehaviorContext<OrderSagaState, PaymentProcessedEvent> context,
        IBehavior<OrderSagaState, PaymentProcessedEvent> next)
    {
        var saga = context.Saga;

        if (!await licenseRepository.ExistsAsync(saga.GameId, saga.UserId, CancellationToken.None))
        {
            var license = new GameLicense(saga.GameId, saga.UserId);
            await licenseRepository.AddAsync(license, CancellationToken.None);

            // Invalida biblioteca do usuário para refletir o novo jogo adquirido
            await cache.RemoveAsync(GetGamesByUserHandler.CacheKeyFor(saga.UserId), CancellationToken.None);
        }

        var order = await orderRepository.GetByIdAsync(saga.CorrelationId, CancellationToken.None);
        if (order is not null)
        {
            order.Complete();
            await orderRepository.UpdateAsync(order, CancellationToken.None);
        }

        await next.Execute(context);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<OrderSagaState, PaymentProcessedEvent, TException> context,
        IBehavior<OrderSagaState, PaymentProcessedEvent> next)
        where TException : Exception => next.Faulted(context);

    public void Probe(ProbeContext context) => context.CreateScope("approve-order");
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
}
