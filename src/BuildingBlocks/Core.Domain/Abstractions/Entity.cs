using Core.Domain.Events;
using Core.Domain.Primitives;

namespace Core.Domain.Abstractions;

/// <summary>
/// Base class for DDD entities with identity, audit fields, soft delete and domain events.
/// Audit and soft-delete fields have private setters: they are written by the unit of work through EF Core,
/// never by application code.
/// </summary>
/// <typeparam name="TId">The identifier type.</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>, IAuditableEntity, ISoftDeletableEntity, IHasDomainEvents
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity(TId id) => Id = id;

#pragma warning disable CS8618 // Required by EF Core.
    protected Entity()
    {
    }
#pragma warning restore CS8618

    public TId Id { get; protected set; }

    public DateTimeOffset Created { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTimeOffset LastModified { get; private set; }

    public string? LastModifiedBy { get; private set; }

    public bool IsDeleted { get; private set; }

    public DateTimeOffset? Deleted { get; private set; }

    public string? DeletedBy { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);

    public void MarkAsDeleted(DateTimeOffset deletedAt, string? deletedBy)
    {
        IsDeleted = true;
        Deleted = deletedAt;
        DeletedBy = deletedBy;
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    public bool Equals(Entity<TId>? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        return ReferenceEquals(this, other) || EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj) => obj is Entity<TId> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
