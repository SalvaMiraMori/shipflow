using Core.Domain.Primitives;

namespace Core.Domain.Abstractions;

/// <summary>
/// Base class for aggregate roots identified by a strongly-typed id.
/// </summary>
/// <remarks>
/// Fixes a bug in the previous Core: the old class re-declared <c>Id</c> with <c>new</c> and called the
/// parameterless base constructor, so <c>BaseEntity.Id</c> stayed <c>null</c> and <c>Equals</c>/<c>GetHashCode</c>
/// threw. There is now a single <c>Id</c>, set through the base constructor.
/// </remarks>
/// <typeparam name="TId">The strongly-typed id.</typeparam>
/// <typeparam name="TIdValue">The primitive value wrapped by the id.</typeparam>
public abstract class AggregateRoot<TId, TIdValue> : Entity<TId>, IAggregateRoot
    where TId : AggregateRootId<TIdValue>
    where TIdValue : notnull
{
    protected AggregateRoot(TId id)
        : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}
