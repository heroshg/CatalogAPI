using Catalog.Domain.Exceptions;

namespace Catalog.Domain.ValueObjects;

public sealed class GameDescription
{
    public const int MaxLength = 300;

    public string Value { get; }

    private GameDescription(string value) => Value = value;

    public static GameDescription From(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Game description cannot be empty.");
        var normalized = description.Trim();
        if (normalized.Length > MaxLength)
            throw new DomainException($"Game description cannot exceed {MaxLength} characters.");
        return new GameDescription(normalized);
    }

    public override bool Equals(object? obj) => obj is GameDescription d && Value == d.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(GameDescription? a, GameDescription? b) => a?.Value == b?.Value;
    public static bool operator !=(GameDescription? a, GameDescription? b) => !(a == b);
    public override string ToString() => Value;
}
