using Catalog.Domain.Exceptions;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities;

public class Game
{
    protected Game() { }

    public Game(string name, string description, decimal price)
    {
        Id          = Guid.NewGuid();
        Name        = GameName.From(name);
        Description = GameDescription.From(description);
        Price       = Money.Of(price);
        IsActive    = true;
        CreatedAt   = DateTime.UtcNow;
        UpdatedAt   = DateTime.UtcNow;
    }

    public Guid            Id          { get; private set; }
    public GameName        Name        { get; private set; } = null!;
    public GameDescription Description { get; private set; } = null!;
    public Money           Price       { get; private set; } = null!;
    public bool            IsActive    { get; private set; }
    public DateTime        CreatedAt   { get; private set; }
    public DateTime        UpdatedAt   { get; private set; }

    public void UpdateDetails(string name, string description, decimal price)
    {
        Name        = GameName.From(name);
        Description = GameDescription.From(description);
        Price       = Money.Of(price);
        UpdatedAt   = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Game is already inactive.");
        IsActive  = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        if (IsActive)
            throw new DomainException("Game is already active.");
        IsActive  = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
