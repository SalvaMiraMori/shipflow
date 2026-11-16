namespace Core.Domain.Events;

/// <summary>
/// Something that happened in the domain that other parts of the same service may react to.
/// No MediatR dependency: dispatching is done by <c>Core.Application</c>.
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }

    DateTimeOffset OccurredOn { get; }
}
