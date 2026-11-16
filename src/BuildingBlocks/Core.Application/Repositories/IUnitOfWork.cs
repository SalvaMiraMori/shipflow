namespace Core.Application.Repositories;

/// <summary>
/// Commits all tracked changes atomically, stamps audit fields and dispatches domain events.
/// No EF Core types leak through this interface (the previous version exposed <c>IDbContextTransaction</c>).
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
