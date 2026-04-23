using Catalog.Domain.Exceptions;

namespace Catalog.Domain.ValueObjects;

public sealed class GameName
{
    public const int MaxLength = 80;

    public string Value { get; }

    private GameName(string value) => Value = value;

    public static GameName From(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Game name cannot be empty.");
        var normalized = name.Trim();
        if (normalized.Length > MaxLength)
            throw new DomainException($"Game name cannot exceed {MaxLength} characters.");
        return new GameName(normalized);
    }

    public override bool Equals(object? obj) => obj is GameName n && Value == n.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(GameName? a, GameName? b) => a?.Value == b?.Value;
    public static bool operator !=(GameName? a, GameName? b) => !(a == b);
    public override string ToString() => Value;
}
