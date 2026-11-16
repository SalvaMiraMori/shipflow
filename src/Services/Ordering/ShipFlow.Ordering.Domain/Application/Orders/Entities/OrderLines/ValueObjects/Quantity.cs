using Core.Domain.Abstractions;
using Core.Domain.Validation;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.Validators;

namespace ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;

/// <summary>
/// Number of units on an order line. Always positive.
/// </summary>
public sealed class Quantity : ValueObject
{
    public const int MinValue = 1;
    public const int MaxValue = 10_000;

    private Quantity(int value) => Value = value;

    private Quantity()
    {
    }

    public int Value { get; private set; }

    public static ErrorOr<Quantity> CreateNew(int value) => QuantityValidator.Instance.ValidateAndCreate(value, v => new Quantity(v));

    public ErrorOr<Quantity> Add(Quantity other) => CreateNew(Value + other.Value);

    public static implicit operator int(Quantity quantity) => quantity.Value;

    public override string ToString() => Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
