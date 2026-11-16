using Core.Application.Messaging;
using Core.Application.Repositories;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Domain.Application.Orders.Specifications;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.Errors;

namespace ShipFlow.Ordering.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(IReadRepository<Order> orderRepository)
    : IQueryHandler<GetOrderByIdQuery, Order>
{
    public async Task<ErrorOr<Order>> HandleAsync(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Create(query.OrderId);
        if (orderId.IsError)
        {
            return orderId.Errors;
        }

        var order = await orderRepository.SingleOrDefaultAsync(new OrderByIdSpecification(orderId.Value), cancellationToken);

        return order is null ? DomainErrors.Order.NotFound(query.OrderId) : order;
    }
}
