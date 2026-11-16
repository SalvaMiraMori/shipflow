using Core.Application.Messaging;
using Core.Application.Repositories;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Domain.Application.Orders.Specifications;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.Errors;

namespace ShipFlow.Ordering.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(IRepository<Order> orderRepository)
    : ICommandHandler<CancelOrderCommand, Order>
{
    public async Task<ErrorOr<Order>> HandleAsync(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Create(command.OrderId);
        if (orderId.IsError)
        {
            return orderId.Errors;
        }

        // Tracked query: the change is saved by the UnitOfWork decorator.
        var order = await orderRepository.SingleOrDefaultAsync(new OrderByIdSpecification(orderId.Value, asNoTracking: false), cancellationToken);
        if (order is null)
        {
            return DomainErrors.Order.NotFound(command.OrderId);
        }

        var cancelled = order.Cancel();
        return cancelled.IsError ? cancelled.Errors : order;
    }
}
