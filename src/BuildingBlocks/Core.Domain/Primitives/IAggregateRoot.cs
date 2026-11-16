namespace Core.Domain.Primitives;

/// <summary>
/// Marker for aggregate roots. Repositories should only be created for types implementing it.
/// </summary>
public interface IAggregateRoot;
