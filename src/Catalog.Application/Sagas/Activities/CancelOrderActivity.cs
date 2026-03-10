using Catalog.Domain.Interfaces;
using FiapCloudGames.Contracts.Events;
using MassTransit;

namespace Catalog.Application.Sagas.Activities;

public class CancelOrderActivity(IOrderRepository orderRepository)
    : IStateMachineActivity<OrderSagaState, PaymentProcessedEvent>
{
    public async Task Execute(
        BehaviorContext<OrderSagaState, PaymentProcessedEvent> context,
        IBehavior<OrderSagaState, PaymentProcessedEvent> next)
    {
        var order = await orderRepository.GetByIdAsync(context.Saga.CorrelationId, CancellationToken.None);
        if (order is not null)
        {
            order.Fail();
            await orderRepository.UpdateAsync(order, CancellationToken.None);
        }

        await next.Execute(context);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<OrderSagaState, PaymentProcessedEvent, TException> context,
        IBehavior<OrderSagaState, PaymentProcessedEvent> next)
        where TException : Exception => next.Faulted(context);

    public void Probe(ProbeContext context) => context.CreateScope("cancel-order");
    public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
}
