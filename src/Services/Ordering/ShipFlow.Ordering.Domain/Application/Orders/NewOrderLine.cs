using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders;

/// <summary>
/// Input for <see cref="Order.Place"/>: an already-validated line that is not yet part of an order.
/// </summary>
public sealed record NewOrderLine(ProductId ProductId, Quantity Quantity, Money UnitPrice);
