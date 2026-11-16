namespace Core.Domain.Primitives;

/// <summary>
/// Entities whose creation and modification are stamped automatically by the unit of work.
/// </summary>
public interface IAuditableEntity
{
    DateTimeOffset Created { get; }

    string? CreatedBy { get; }

    DateTimeOffset LastModified { get; }

    string? LastModifiedBy { get; }
}
