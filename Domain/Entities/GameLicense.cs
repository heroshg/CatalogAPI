namespace CatalogAPI.Domain.Entities;

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
}
