using Core.Domain.Abstractions;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines;

/// <summary>
/// A product, quantity and unit price inside an <see cref="Order"/>. Only the aggregate can create or change lines.
/// </summary>
public sealed class OrderLine : Entity<OrderLineId>
{
    internal OrderLine(OrderLineId id, ProductId productId, Quantity quantity, Money unitPrice)
        : base(id)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

#pragma warning disable CS8618 // Required by EF Core.
    private OrderLine()
    {
    }
#pragma warning restore CS8618

    public ProductId ProductId { get; private set; }

    public Quantity Quantity { get; private set; }

    public Money UnitPrice { get; private set; }

    public Money LineTotal => UnitPrice.Multiply(Quantity.Value);
}
