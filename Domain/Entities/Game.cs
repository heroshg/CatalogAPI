namespace CatalogAPI.Domain.Entities;

public class Game
{
    protected Game() { }

    public Game(string name, string description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Game name cannot be empty.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Game description cannot be empty.");
        if (price < 0)
            throw new DomainException("Game price cannot be negative.");

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
