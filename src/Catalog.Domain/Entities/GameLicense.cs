namespace Catalog.Domain.Entities;

public class GameLicense
{
    protected GameLicense() { }

    public GameLicense(Guid gameId, Guid userId)
    {
        Id = Guid.NewGuid();
        GameId = gameId;
        UserId = userId;
        AcquiredAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public Guid GameId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime AcquiredAt { get; private set; }

    public static GameLicense Reconstitute(Guid id, Guid gameId, Guid userId, DateTime acquiredAt) =>
        new() { Id = id, GameId = gameId, UserId = userId, AcquiredAt = acquiredAt };
}
