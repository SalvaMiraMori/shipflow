using Core.Domain.Abstractions;
using Core.Domain.Validation;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders.Validators;

namespace ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

/// <summary>
/// Identifier of the <see cref="Order"/> aggregate.
/// </summary>
public sealed class OrderId : AggregateRootId<Guid>
{
    private OrderId(Guid value) => Value = value;

    private OrderId()
    {
    }

    public override Guid Value { get; protected set; }

    public static OrderId CreateUnique() => new(Guid.NewGuid());

    /// <summary>
    /// Creates the id from an existing value; <see cref="Guid.Empty"/> is rejected.
    /// </summary>
    public static ErrorOr<OrderId> Create(Guid value) => OrderIdValidator.Instance.ValidateAndCreate(value, v => new OrderId(v));

    public static implicit operator Guid(OrderId id) => id.Value;
}
