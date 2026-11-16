using Core.Specification;
using Core.Specification.Builder;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Specifications;

/// <summary>
/// One order with its lines (lines are owned by the aggregate, so EF Core loads them automatically).
/// </summary>
public sealed class OrderByIdSpecification : SingleResultSpecification<Order>
{
    public OrderByIdSpecification(OrderId orderId, bool asNoTracking = true)
    {
        Query.IdEquals(orderId)
            .AsNoTracking(asNoTracking)
            .TagWith(nameof(OrderByIdSpecification));
    }
}
