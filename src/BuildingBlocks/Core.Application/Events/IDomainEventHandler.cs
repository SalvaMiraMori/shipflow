using Core.Domain.Events;

namespace Core.Application.Events;

/// <summary>
/// Reacts to a domain event raised by an aggregate. Runs in-process after the unit of work has committed.
/// </summary>
public interface IDomainEventHandler<in TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
}
