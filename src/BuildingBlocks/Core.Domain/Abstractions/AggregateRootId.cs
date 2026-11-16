using Core.Domain.Primitives;

namespace Core.Domain.Abstractions;

/// <summary>
/// Strongly-typed identifier of an aggregate root (e.g. <c>OrderId</c> wrapping a <see cref="Guid"/>).
/// </summary>
/// <remarks>
/// The previous version implemented <c>IAggregateRoot</c> by mistake (an id is not an aggregate); it now implements
/// <see cref="IAggregateRootId"/>.
/// </remarks>
public abstract class AggregateRootId<TValue> : EntityId<TValue>, IAggregateRootId
    where TValue : notnull;
