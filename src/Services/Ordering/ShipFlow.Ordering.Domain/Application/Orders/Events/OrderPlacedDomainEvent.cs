using Core.Domain.Events;

namespace ShipFlow.Ordering.Domain.Application.Orders.Events;

/// <summary>
/// Raised when an order is placed. Becomes the <c>OrderPlaced</c> integration event in Phase 3 (outbox).
/// Carries primitives only so it can be serialised without the domain model.
/// </summary>
public sealed record OrderPlacedDomainEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount, string Currency) : DomainEvent;
