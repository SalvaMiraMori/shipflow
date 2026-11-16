namespace Core.Application.Repositories;

/// <summary>
/// Change-tracking operations. Nothing is written until <see cref="IUnitOfWork.SaveChangesAsync"/> runs,
/// which the command decorator does once per successful command; that is why there is no SaveChanges here.
/// </summary>
public interface IWriteRepository<T>
    where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    /// <summary>Removes the entity. Soft-deletable entities are flagged as deleted instead.</summary>
    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);
}
