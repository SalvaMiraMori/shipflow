using Core.Application.Messaging;
using Core.Application.Repositories;
using Core.Domain.Common;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Domain.Application.Orders.Enums;
using ShipFlow.Ordering.Domain.Application.Orders.Filters;
using ShipFlow.Ordering.Domain.Application.Orders.Specifications;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

namespace ShipFlow.Ordering.Application.Orders.Queries.GetOrders;

/// <summary>
/// Unlike EcoGreen's list handlers it does not catch exceptions and turn them into "Unexpected" errors: infrastructure
/// failures bubble up to the global exception handler (500 Problem Details) and are traced.
/// </summary>
public sealed class GetOrdersQueryHandler(IReadRepository<Order> orderRepository)
    : IQueryHandler<GetOrdersQuery, PaginatedResult<Order>>
{
    public async Task<ErrorOr<PaginatedResult<Order>>> HandleAsync(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        CustomerId? customerId = null;
        if (query.CustomerId is { } rawCustomerId)
        {
            var result = CustomerId.Create(rawCustomerId);
            if (result.IsError)
            {
                errors.AddRange(result.Errors);
            }
            else
            {
                customerId = result.Value;
            }
        }

        OrderStatus? status = null;
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var result = OrderStatus.FromName(query.Status);
            if (result.IsError)
            {
                errors.AddRange(result.Errors);
            }
            else
            {
                status = result.Value;
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        var filter = OrderFilter.FromQuery(query);
        return await orderRepository.PageAsync(new OrdersSpecification(customerId, status, filter), filter, cancellationToken);
    }
}
