using Catalog.Application.Sagas;
using MassTransit;

namespace Catalog.Infrastructure.Persistence.DynamoDB.Sagas;

internal sealed class OrderSagaConsumeContext<TMessage>
    : SagaConsumeContext<OrderSagaState, TMessage>
    where TMessage : class
{
    private readonly ConsumeContext<TMessage> _inner;

    public OrderSagaConsumeContext(ConsumeContext<TMessage> inner, OrderSagaState saga)
    {
        _inner = inner;
        Saga   = saga;
    }

    public OrderSagaState Saga { get; }
    public bool IsCompleted { get; private set; }
    public Task SetCompleted() { IsCompleted = true; return Task.CompletedTask; }

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
}
