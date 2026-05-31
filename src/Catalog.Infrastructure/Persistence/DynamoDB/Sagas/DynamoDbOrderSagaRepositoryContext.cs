using Amazon.DynamoDBv2.DataModel;
using Catalog.Application.Sagas;
using Catalog.Infrastructure.Persistence.DynamoDB.Documents;
using MassTransit;
using MassTransit.Saga;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Sagas;

public sealed class DynamoDbOrderSagaRepositoryContext<TMessage>
    : SagaRepositoryContext<OrderSagaState, TMessage>
    where TMessage : class
{
    private readonly ConsumeContext<TMessage> _inner;
    private readonly IDynamoDBContext _db;

    public DynamoDbOrderSagaRepositoryContext(ConsumeContext<TMessage> consumeContext, IDynamoDBContext db)
    {
        _inner = consumeContext;
        _db    = db;
    }

    Task<SagaConsumeContext<OrderSagaState, T>> ISagaConsumeContextFactory<OrderSagaState>.CreateSagaConsumeContext<T>(
        ConsumeContext<T> context, OrderSagaState instance, SagaConsumeContextMode mode)
        where T : class =>
        Task.FromResult<SagaConsumeContext<OrderSagaState, T>>(
            new OrderSagaConsumeContext<T>(context, instance));

    async Task<SagaConsumeContext<OrderSagaState, TMessage>?> SagaRepositoryContext<OrderSagaState, TMessage>.Load(Guid correlationId)
    {
        var doc = await _db.LoadAsync<OrderSagaStateDocument>(correlationId.ToString(), _inner.CancellationToken);
        if (doc is null) return null;
        return new OrderSagaConsumeContext<TMessage>(_inner, doc.ToState());
    }

    async Task<SagaConsumeContext<OrderSagaState, TMessage>> SagaRepositoryContext<OrderSagaState, TMessage>.Add(OrderSagaState instance)
    {
        await PersistAsync(instance);
        return new OrderSagaConsumeContext<TMessage>(_inner, instance);
    }

    async Task<SagaConsumeContext<OrderSagaState, TMessage>> SagaRepositoryContext<OrderSagaState, TMessage>.Insert(OrderSagaState instance)
    {
        await PersistAsync(instance);
        return new OrderSagaConsumeContext<TMessage>(_inner, instance);
    }

    Task SagaRepositoryContext<OrderSagaState, TMessage>.Save(SagaConsumeContext<OrderSagaState> context)    => PersistAsync(context.Saga);
    Task SagaRepositoryContext<OrderSagaState, TMessage>.Update(SagaConsumeContext<OrderSagaState> context)  => PersistAsync(context.Saga);
    Task SagaRepositoryContext<OrderSagaState, TMessage>.Discard(SagaConsumeContext<OrderSagaState> context) => Task.CompletedTask;
    Task SagaRepositoryContext<OrderSagaState, TMessage>.Undo(SagaConsumeContext<OrderSagaState> context)    => Task.CompletedTask;

    async Task SagaRepositoryContext<OrderSagaState, TMessage>.Delete(SagaConsumeContext<OrderSagaState> context) =>
        await _db.DeleteAsync(OrderSagaStateDocument.FromState(context.Saga), _inner.CancellationToken);

    CancellationToken PipeContext.CancellationToken                                        => _inner.CancellationToken;
    bool PipeContext.HasPayloadType(Type payloadType)                                      => _inner.HasPayloadType(payloadType);
    bool PipeContext.TryGetPayload<T>(out T payload)                                       => _inner.TryGetPayload(out payload!);
    T PipeContext.GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)                     => _inner.GetOrAddPayload(payloadFactory);
    T PipeContext.AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        => _inner.AddOrUpdatePayload(addFactory, updateFactory);

    Guid? MessageContext.MessageId         => _inner.MessageId;
    Guid? MessageContext.RequestId         => _inner.RequestId;
    Guid? MessageContext.CorrelationId     => _inner.CorrelationId;
    Guid? MessageContext.ConversationId    => _inner.ConversationId;
    Guid? MessageContext.InitiatorId       => _inner.InitiatorId;
    DateTime? MessageContext.ExpirationTime => _inner.ExpirationTime;
    Uri? MessageContext.SourceAddress      => _inner.SourceAddress;
    Uri? MessageContext.DestinationAddress => _inner.DestinationAddress;
    Uri? MessageContext.ResponseAddress    => _inner.ResponseAddress;
    Uri? MessageContext.FaultAddress       => _inner.FaultAddress;
    DateTime? MessageContext.SentTime      => _inner.SentTime;
    Headers MessageContext.Headers         => _inner.Headers;
    HostInfo MessageContext.Host           => _inner.Host;

    ReceiveContext ConsumeContext.ReceiveContext                                    => _inner.ReceiveContext;
    MassTransit.SerializerContext ConsumeContext.SerializerContext                  => _inner.SerializerContext;
    IEnumerable<string> ConsumeContext.SupportedMessageTypes                       => _inner.SupportedMessageTypes;
    Task ConsumeContext.ConsumeCompleted                                            => _inner.ConsumeCompleted;

    bool ConsumeContext.HasMessageType(Type messageType)                           => _inner.HasMessageType(messageType);
#pragma warning disable CS8769
    bool ConsumeContext.TryGetMessage<T>(out ConsumeContext<T>? consumeCtx) => _inner.TryGetMessage(out consumeCtx);
#pragma warning restore CS8769
    void ConsumeContext.AddConsumeTask(Task task)                                   => _inner.AddConsumeTask(task);

    Task ConsumeContext.NotifyConsumed<T>(ConsumeContext<T> ctx, TimeSpan duration, string consumerType)
        => _inner.NotifyConsumed(ctx, duration, consumerType);
    Task ConsumeContext.NotifyFaulted<T>(ConsumeContext<T> ctx, TimeSpan elapsed, string consumer, Exception ex)
        => _inner.NotifyFaulted(ctx, elapsed, consumer, ex);

    Task ConsumeContext.RespondAsync<T>(T message)                                 => _inner.RespondAsync(message);
    Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)  => _inner.RespondAsync(message, sendPipe);
    Task ConsumeContext.RespondAsync<T>(T message, IPipe<SendContext> sendPipe)     => _inner.RespondAsync(message, sendPipe);
    Task ConsumeContext.RespondAsync(object message)                               => _inner.RespondAsync(message);
    Task ConsumeContext.RespondAsync(object message, Type messageType)             => _inner.RespondAsync(message, messageType);
    Task ConsumeContext.RespondAsync(object message, IPipe<SendContext> pipe)       => _inner.RespondAsync(message, pipe);
    Task ConsumeContext.RespondAsync(object message, Type messageType, IPipe<SendContext> pipe) => _inner.RespondAsync(message, messageType, pipe);
    Task ConsumeContext.RespondAsync<T>(object values)                             => _inner.RespondAsync<T>(values);
    Task ConsumeContext.RespondAsync<T>(object values, IPipe<SendContext<T>> pipe)  => _inner.RespondAsync<T>(values, pipe);
    Task ConsumeContext.RespondAsync<T>(object values, IPipe<SendContext> pipe)     => _inner.RespondAsync<T>(values, pipe);
    void ConsumeContext.Respond<T>(T message)                                      => _inner.Respond(message);

    TMessage ConsumeContext<TMessage>.Message                                      => _inner.Message;
    Task ConsumeContext<TMessage>.NotifyConsumed(TimeSpan duration, string consumerType)  => _inner.NotifyConsumed(duration, consumerType);
    Task ConsumeContext<TMessage>.NotifyFaulted(TimeSpan elapsed, string consumer, Exception ex) => _inner.NotifyFaulted(elapsed, consumer, ex);

    Task IPublishEndpoint.Publish<T>(T msg, CancellationToken ct)                  => _inner.Publish(msg, ct);
    Task IPublishEndpoint.Publish<T>(T msg, IPipe<PublishContext<T>> pipe, CancellationToken ct)  => _inner.Publish(msg, pipe, ct);
    Task IPublishEndpoint.Publish<T>(T msg, IPipe<PublishContext> pipe, CancellationToken ct)     => _inner.Publish(msg, pipe, ct);
    Task IPublishEndpoint.Publish(object msg, CancellationToken ct)                => _inner.Publish(msg, ct);
    Task IPublishEndpoint.Publish(object msg, IPipe<PublishContext> pipe, CancellationToken ct)   => _inner.Publish(msg, pipe, ct);
    Task IPublishEndpoint.Publish(object msg, Type type, CancellationToken ct)     => _inner.Publish(msg, type, ct);
    Task IPublishEndpoint.Publish(object msg, Type type, IPipe<PublishContext> pipe, CancellationToken ct) => _inner.Publish(msg, type, pipe, ct);
    Task IPublishEndpoint.Publish<T>(object values, CancellationToken ct)          => _inner.Publish<T>(values, ct);
    Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext<T>> pipe, CancellationToken ct) => _inner.Publish<T>(values, pipe, ct);
    Task IPublishEndpoint.Publish<T>(object values, IPipe<PublishContext> pipe, CancellationToken ct) => _inner.Publish<T>(values, pipe, ct);

    Task<ISendEndpoint> ISendEndpointProvider.GetSendEndpoint(Uri address) => _inner.GetSendEndpoint(address);

    ConnectHandle IPublishObserverConnector.ConnectPublishObserver(IPublishObserver observer) => _inner.ConnectPublishObserver(observer);

    ConnectHandle ISendObserverConnector.ConnectSendObserver(ISendObserver observer) => _inner.ConnectSendObserver(observer);

    private Task PersistAsync(OrderSagaState state) =>
        _db.SaveAsync(OrderSagaStateDocument.FromState(state), _inner.CancellationToken);
}
