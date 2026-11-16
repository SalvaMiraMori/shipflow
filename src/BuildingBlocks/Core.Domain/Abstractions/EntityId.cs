namespace Core.Domain.Abstractions;

/// <summary>
/// Base class for strongly-typed identifiers (e.g. <c>OrderLineId</c>, or a reference to another context's
/// <c>CustomerId</c>). Prevents passing an order id where a customer id is expected.
/// </summary>
/// <typeparam name="TValue">The primitive type that is stored in the database.</typeparam>
public abstract class EntityId<TValue> : ValueObject
    where TValue : notnull
{
    public abstract TValue Value { get; protected set; }

    public override string ToString() => Value.ToString() ?? string.Empty;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
