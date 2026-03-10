namespace FiapCloudGames.Contracts.Events;

public record OrderPlacedEvent(
    Guid OrderId,
    Guid UserId,
    string UserEmail,
    Guid GameId,
    string GameName,
    decimal Price);
