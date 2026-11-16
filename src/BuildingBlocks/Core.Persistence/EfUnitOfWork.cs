using Core.Application.Events;
using Core.Application.Repositories;
using Core.Application.Services;
using Core.Domain.Events;
using Core.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Persistence;

/// <summary>
/// EF Core unit of work. On every save it:
/// <list type="number">
/// <item>stamps <see cref="IAuditableEntity"/> fields (who and when),</item>
/// <item>turns deletes of <see cref="ISoftDeletableEntity"/> into soft deletes,</item>
/// <item>saves, and then dispatches the domain events raised by the tracked entities.</item>
/// </list>
/// </summary>
/// <remarks>
/// Fixes from the previous Core/EcoGreen version: it no longer disposes the DbContext it does not own (the DI
/// container does), it awaits SaveChanges (the old <c>BaseRepository.SaveChangesAsync</c> fired and forgot), uses
/// <see cref="TimeProvider"/> instead of a custom date provider, and dispatches domain events.
/// </remarks>
public class EfUnitOfWork<TContext>(
    TContext dbContext,
    TimeProvider timeProvider,
    ICurrentUserProvider currentUser,
    IDomainEventDispatcher domainEventDispatcher) : IUnitOfWork
    where TContext : DbContext
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();
        var userId = currentUser.UserId;

        ApplyAuditing(now, userId);
        ApplySoftDelete(now, userId);

        var domainEvents = CollectDomainEvents();
        var written = await dbContext.SaveChangesAsync(cancellationToken);

        // Dispatched after commit: handlers only see state that is really persisted. Integration events that must
        // not be lost go through the transactional outbox (Phase 3) instead of here.
        await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

        return written;
    }

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new EfTransaction(transaction);
    }

    private void ApplyAuditing(DateTimeOffset now, string? userId)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditableEntity.Created)).CurrentValue = now;
                entry.Property(nameof(IAuditableEntity.CreatedBy)).CurrentValue = userId;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Property(nameof(IAuditableEntity.LastModified)).CurrentValue = now;
                entry.Property(nameof(IAuditableEntity.LastModifiedBy)).CurrentValue = userId;
            }
        }
    }

    private void ApplySoftDelete(DateTimeOffset now, string? userId)
    {
        foreach (var entry in dbContext.ChangeTracker.Entries<ISoftDeletableEntity>().Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.MarkAsDeleted(now, userId);
        }
    }

    private List<IDomainEvent> CollectDomainEvents()
    {
        var entities = dbContext.ChangeTracker.Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Count > 0)
            .ToList();

        var events = entities.SelectMany(entity => entity.DomainEvents).ToList();
        entities.ForEach(entity => entity.ClearDomainEvents());

        return events;
    }

    private sealed class EfTransaction(IDbContextTransaction transaction) : IUnitOfWorkTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken = default) => transaction.CommitAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken = default) => transaction.RollbackAsync(cancellationToken);

        public ValueTask DisposeAsync() => transaction.DisposeAsync();
    }
}
