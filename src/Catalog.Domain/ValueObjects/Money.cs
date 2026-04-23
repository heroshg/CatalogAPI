using Catalog.Domain.Exceptions;

namespace Catalog.Domain.ValueObjects;

public sealed class Money
{
    public decimal Amount { get; }

    private Money(decimal amount) => Amount = amount;

    public static Money Of(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Price cannot be negative.");
        return new Money(Math.Round(amount, 2, MidpointRounding.AwayFromZero));
    }

    public static Money Zero => new(0m);

    public override bool Equals(object? obj) => obj is Money m && Amount == m.Amount;
    public override int GetHashCode() => Amount.GetHashCode();
    public static bool operator ==(Money? a, Money? b) => a?.Amount == b?.Amount;
    public static bool operator !=(Money? a, Money? b) => !(a == b);
    public override string ToString() => Amount.ToString("F2");
}
