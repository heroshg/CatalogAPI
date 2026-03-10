using Catalog.Application.Sagas.Activities;
using FiapCloudGames.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Sagas;

public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public State OrderPlaced { get; private set; } = null!;
    public State Completed { get; private set; } = null!;
    public State Failed { get; private set; } = null!;

    public Event<OrderPlacedEvent> OrderPlacedEvent { get; private set; } = null!;
    public Event<PaymentProcessedEvent> PaymentProcessedEvent { get; private set; } = null!;

    public OrderSagaStateMachine(ILogger<OrderSagaStateMachine> logger)
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderPlacedEvent, x => x.CorrelateById(ctx => ctx.Message.OrderId));
        Event(() => PaymentProcessedEvent, x => x.CorrelateById(ctx => ctx.Message.OrderId));

        Initially(
            When(OrderPlacedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.UserEmail = ctx.Message.UserEmail;
                    ctx.Saga.GameId = ctx.Message.GameId;
                    ctx.Saga.GameName = ctx.Message.GameName;
                    ctx.Saga.Price = ctx.Message.Price;
                    ctx.Saga.CreatedAt = DateTime.UtcNow;
                    logger.LogInformation("[SAGA] OrderPlaced → state=OrderPlaced OrderId={OrderId}", ctx.Saga.CorrelationId);
                })
                .TransitionTo(OrderPlaced));

        During(OrderPlaced,
            When(PaymentProcessedEvent, ctx => ctx.Message.Status == "Approved")
                .Then(ctx => logger.LogInformation(
                    "[SAGA] PaymentApproved → granting license. OrderId={OrderId}", ctx.Saga.CorrelationId))
                .Activity(x => x.OfType<ApproveOrderActivity>())
                .TransitionTo(Completed)
                .Finalize(),

            When(PaymentProcessedEvent, ctx => ctx.Message.Status != "Approved")
                .Then(ctx => logger.LogInformation(
                    "[SAGA] PaymentRejected → compensation. OrderId={OrderId}", ctx.Saga.CorrelationId))
                .Activity(x => x.OfType<CancelOrderActivity>())
                .Publish(ctx => new OrderCancelledEvent(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.UserId,
                    ctx.Saga.UserEmail,
                    ctx.Saga.GameId,
                    ctx.Saga.GameName,
                    ctx.Saga.Price))
                .TransitionTo(Failed)
                .Finalize());

        SetCompletedWhenFinalized();
    }
}
