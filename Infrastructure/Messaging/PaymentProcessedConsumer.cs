using CatalogAPI.Domain.Entities;
using CatalogAPI.Domain.Interfaces;
using FiapCloudGames.Contracts.Events;
using MassTransit;

namespace CatalogAPI.Infrastructure.Messaging;

public class PaymentProcessedConsumer(
    IGameLicenseRepository licenseRepository,
    ILogger<PaymentProcessedConsumer> logger)
    : IConsumer<PaymentProcessedEvent>
{
    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("PaymentProcessedEvent received. OrderId={OrderId} Status={Status}", evt.OrderId, evt.Status);

        if (evt.Status != "Approved")
        {
            logger.LogWarning("Payment rejected for OrderId={OrderId}. Game license NOT granted.", evt.OrderId);
            return;
        }

        if (await licenseRepository.ExistsAsync(evt.GameId, evt.UserId, context.CancellationToken))
        {
            logger.LogWarning("User {UserId} already owns game {GameId}. Skipping.", evt.UserId, evt.GameId);
            return;
        }

        var license = new GameLicense(evt.GameId, evt.UserId);
        await licenseRepository.AddAsync(license, context.CancellationToken);

        logger.LogInformation("Game license granted. UserId={UserId} GameId={GameId}", evt.UserId, evt.GameId);
    }
}
