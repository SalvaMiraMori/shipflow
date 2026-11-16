using Core.Domain.Abstractions;
using Core.Domain.Validation;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders.Validators;

namespace ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

/// <summary>
/// Reference to the customer who placed the order (owned by a future Customers context).
/// </summary>
public sealed class CustomerId : EntityId<Guid>
{
    private CustomerId(Guid value) => Value = value;

    private CustomerId()
    {
    }

    public override Guid Value { get; protected set; }

    public static CustomerId CreateUnique() => new(Guid.NewGuid());

    /// <summary>
    /// Creates the id from an existing value; <see cref="Guid.Empty"/> is rejected.
    /// </summary>
    public static ErrorOr<CustomerId> Create(Guid value) => CustomerIdValidator.Instance.ValidateAndCreate(value, v => new CustomerId(v));

    public static implicit operator Guid(CustomerId id) => id.Value;
}
