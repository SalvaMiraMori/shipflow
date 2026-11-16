using Core.Domain.Abstractions;
using Core.Domain.Validation;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;
using ShipFlow.Ordering.Domain.Application.Orders.Enums;
using ShipFlow.Ordering.Domain.Application.Orders.Events;
using ShipFlow.Ordering.Domain.Application.Orders.Validators;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.Errors;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders;

/// <summary>
/// Order aggregate (spec 001). Invariants: at least one line, a single currency, one line per product
/// (duplicates are merged by summing quantities), status changes only through methods.
/// </summary>
public sealed class Order : AggregateRoot<OrderId, Guid>
{
    private readonly List<OrderLine> _lines = [];

    private Order(OrderId id, CustomerId customerId, Address shippingAddress, DateTimeOffset placedAt)
        : base(id)
    {
        CustomerId = customerId;
        ShippingAddress = shippingAddress;
        PlacedAt = placedAt;
        Status = OrderStatus.Placed;
    }

#pragma warning disable CS8618 // Required by EF Core.
    private Order()
    {
    }
#pragma warning restore CS8618

    public CustomerId CustomerId { get; private set; }

    public Address ShippingAddress { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTimeOffset PlacedAt { get; private set; }

    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    /// <summary>
    /// Sum of <c>quantity × unitPrice</c> of every line. Derived, never stored.
    /// </summary>
    public Money Total => _lines.Count == 0
        ? Money.Zero(string.Empty)
        : Money.FromTrusted(_lines.Sum(line => line.LineTotal.Amount), _lines[0].UnitPrice.Currency);

    /// <summary>
    /// Places a new order and raises <see cref="OrderPlacedDomainEvent"/>.
    /// </summary>
    public static ErrorOr<Order> Place(
        CustomerId customerId,
        Address shippingAddress,
        IReadOnlyCollection<NewOrderLine> lines,
        DateTimeOffset placedAt)
    {
        var mergedLines = MergeDuplicateProducts(lines);
        if (mergedLines.IsError)
        {
            return mergedLines.Errors;
        }

        var order = new Order(OrderId.CreateUnique(), customerId, shippingAddress, placedAt);
        foreach (var line in mergedLines.Value)
        {
            order._lines.Add(new OrderLine(OrderLineId.CreateUnique(), line.ProductId, line.Quantity, line.UnitPrice));
        }

        var validated = OrderValidator.Instance.ValidateToErrorOr(order);
        if (validated.IsError)
        {
            return validated.Errors;
        }

        order.RaiseDomainEvent(new OrderPlacedDomainEvent(order.Id, customerId, order.Total.Amount, order.Total.Currency));
        return order;
    }

    /// <summary>
    /// Cancels a placed order. Shipped or already cancelled orders cannot be cancelled.
    /// </summary>
    public ErrorOr<Success> Cancel()
    {
        if (Status != OrderStatus.Placed)
        {
            return DomainErrors.Order.InvalidStatusTransition(Status.Name, OrderStatus.Cancelled.Name);
        }

        Status = OrderStatus.Cancelled;
        return Result.Success;
    }

    private static ErrorOr<List<NewOrderLine>> MergeDuplicateProducts(IEnumerable<NewOrderLine> lines)
    {
        var merged = new List<NewOrderLine>();
        var errors = new List<Error>();

        foreach (var group in lines.GroupBy(line => line.ProductId))
        {
            var first = group.First();
            if (group.Any(line => line.UnitPrice != first.UnitPrice))
            {
                errors.Add(DomainErrors.Order.ConflictingUnitPrice(first.ProductId));
                continue;
            }

            var quantity = first.Quantity;
            foreach (var line in group.Skip(1))
            {
                var sum = quantity.Add(line.Quantity);
                if (sum.IsError)
                {
                    errors.AddRange(sum.Errors);
                    break;
                }

                quantity = sum.Value;
            }

            merged.Add(first with { Quantity = quantity });
        }

        return errors.Count > 0 ? errors : merged;
    }
}
