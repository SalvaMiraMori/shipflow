using Core.Specification;
using Core.Specification.Builder;
using ShipFlow.Ordering.Domain.Application.Orders.Enums;
using ShipFlow.Ordering.Domain.Application.Orders.Filters;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Specifications;

/// <summary>
/// Read-only list of orders filtered by customer and status. Paging is applied by the repository from the filter.
/// </summary>
public sealed class OrdersSpecification : Specification<Order>
{
    public OrdersSpecification(CustomerId? customerId, OrderStatus? status, OrderFilter filter)
    {
        Query.CustomerIdEquals(customerId)
            .StatusEquals(status)
            .ApplyOrdering(filter)
            .AsNoTracking()
            .TagWith(nameof(OrdersSpecification));
    }
}
