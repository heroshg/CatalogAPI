using FiapCloudGames.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Consumers;

public class OrderCancelledEventConsumer(ILogger<OrderCancelledEventConsumer> logger)
    : IConsumer<OrderCancelledEvent>
{
    public Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        var e = context.Message;
        logger.LogInformation(
            "[OrderCancelled] OrderId={OrderId} UserId={UserId} Email={Email} Game={GameName} Price={Price}",
            e.OrderId, e.UserId, e.UserEmail, e.GameName, e.Price);
        return Task.CompletedTask;
    }
}
