using Core.Domain.Abstractions;
using Core.Domain.Validation;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.Validators;

namespace ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;

/// <summary>
/// Identifier of an <see cref="OrderLine"/> inside its order.
/// </summary>
public sealed class OrderLineId : EntityId<Guid>
{
    private OrderLineId(Guid value) => Value = value;

    private OrderLineId()
    {
    }

    public override Guid Value { get; protected set; }

    public static OrderLineId CreateUnique() => new(Guid.NewGuid());

    /// <summary>
    /// Creates the id from an existing value; <see cref="Guid.Empty"/> is rejected.
    /// </summary>
    public static ErrorOr<OrderLineId> Create(Guid value) => OrderLineIdValidator.Instance.ValidateAndCreate(value, v => new OrderLineId(v));

    public static implicit operator Guid(OrderLineId id) => id.Value;
}
