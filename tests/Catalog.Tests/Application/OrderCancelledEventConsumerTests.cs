using Catalog.Application.Consumers;
using FiapCloudGames.Contracts.Events;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Catalog.Tests.Application;

public class OrderCancelledEventConsumerTests
{
    [Fact]
    public async Task Consume_LogsEventDetails()
    {
        var logger = new Mock<ILogger<OrderCancelledEventConsumer>>();
        var sut = new OrderCancelledEventConsumer(logger.Object);

        var orderId = Guid.NewGuid();
        var userId  = Guid.NewGuid();
        var gameId  = Guid.NewGuid();
        var evt = new OrderCancelledEvent(orderId, userId, "player@example.com", gameId, "Hades", 59.90m);

        var ctx = new Mock<ConsumeContext<OrderCancelledEvent>>();
        ctx.Setup(c => c.Message).Returns(evt);

        await sut.Consume(ctx.Object);

        logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(orderId.ToString())),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

}
