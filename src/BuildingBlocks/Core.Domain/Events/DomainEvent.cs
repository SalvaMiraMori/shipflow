namespace Core.Domain.Events;

/// <summary>
/// Base record for domain events. Records give value equality and immutability for free.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public DateTimeOffset OccurredOn { get; init; } = DateTimeOffset.UtcNow;
}
