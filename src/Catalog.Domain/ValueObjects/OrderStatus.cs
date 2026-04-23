namespace Catalog.Domain.ValueObjects;

public sealed class OrderStatus
{
    public static readonly OrderStatus Pending   = new("Pending");
    public static readonly OrderStatus Completed = new("Completed");
    public static readonly OrderStatus Failed    = new("Failed");

    public string Value { get; }

    private OrderStatus(string value) => Value = value;

    public override bool Equals(object? obj) => obj is OrderStatus s && Value == s.Value;
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(OrderStatus? a, OrderStatus? b) => a?.Value == b?.Value;
    public static bool operator !=(OrderStatus? a, OrderStatus? b) => !(a == b);
    public override string ToString() => Value;
}
