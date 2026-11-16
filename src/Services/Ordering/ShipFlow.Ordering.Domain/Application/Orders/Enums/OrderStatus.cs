using Core.Domain.Abstractions;

namespace ShipFlow.Ordering.Domain.Application.Orders.Enums;

/// <summary>
/// Lifecycle of an order. Following the EcoGreen convention, the class is named without an "Enum" suffix.
/// Later phases add transitions (StockReserved, Shipped...) as the saga is built.
/// </summary>
public sealed class OrderStatus : Enumeration<OrderStatus>
{
    public static readonly OrderStatus Placed = new(1, nameof(Placed));
    public static readonly OrderStatus Cancelled = new(2, nameof(Cancelled));
    public static readonly OrderStatus Shipped = new(3, nameof(Shipped));

    private OrderStatus(int value, string name)
        : base(value, name)
    {
    }

    private OrderStatus()
    {
    }
}
