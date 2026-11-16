namespace Core.Domain.Primitives;

/// <summary>
/// Entities that are flagged as deleted instead of being removed. The unit of work converts deletes into updates.
/// </summary>
public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }

    DateTimeOffset? Deleted { get; }

    string? DeletedBy { get; }

    void MarkAsDeleted(DateTimeOffset deletedAt, string? deletedBy);
}
