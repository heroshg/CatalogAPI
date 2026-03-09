namespace FiapCloudGames.Contracts.Events;

public record PaymentProcessedEvent(
    Guid OrderId,
    Guid UserId,
    string UserEmail,
    Guid GameId,
    string GameName,
    decimal Price,
    string Status);
