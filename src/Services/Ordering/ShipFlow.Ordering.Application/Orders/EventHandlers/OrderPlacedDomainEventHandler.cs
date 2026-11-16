using Core.Application.Events;
using Microsoft.Extensions.Logging;
using ShipFlow.Ordering.Domain.Application.Orders.Events;

namespace ShipFlow.Ordering.Application.Orders.EventHandlers;

/// <summary>
/// Phase 1: records the event. Phase 3 replaces this with writing an <c>OrderPlaced</c> integration event to the
/// transactional outbox, so Inventory can reserve stock.
/// </summary>
internal sealed class OrderPlacedDomainEventHandler(ILogger<OrderPlacedDomainEventHandler> logger)
    : IDomainEventHandler<OrderPlacedDomainEvent>
{
    public Task HandleAsync(OrderPlacedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Order {OrderId} placed by customer {CustomerId} for {Total} {Currency}",
            domainEvent.OrderId,
            domainEvent.CustomerId,
            domainEvent.TotalAmount,
            domainEvent.Currency);

        return Task.CompletedTask;
    }
}
