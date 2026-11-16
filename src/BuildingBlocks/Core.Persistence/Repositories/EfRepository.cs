using Core.Application.Repositories;
using Core.Domain.Common;
using Core.Domain.Primitives;
using Core.Specification;
using Core.Specification.Evaluators;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence.Repositories;

/// <summary>
/// Generic EF Core repository driven by specifications. Equivalent of EcoGreen's <c>EcogreenBaseRepository</c>, with
/// these fixes: no swallowed exceptions, no <c>ToBlockingEnumerable</c>, paging applied once (count + page in two
/// round-trips, both using the same criteria), no SaveChanges here, and <c>FindAsync</c> returns a tracked entity.
/// </summary>
public class EfRepository<T, TContext>(TContext dbContext, ISpecificationEvaluator evaluator) : IRepository<T>
    where T : class, IAggregateRoot
    where TContext : DbContext
{
    protected TContext DbContext { get; } = dbContext;

    protected DbSet<T> DbSet { get; } = dbContext.Set<T>();

    protected ISpecificationEvaluator Evaluator { get; } = evaluator;

    // ---- Write -----------------------------------------------------------------------------------------------

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await DbSet.AddAsync(entity, cancellationToken);

    public virtual Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) =>
        DbSet.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(T entity) => DbSet.Update(entity);

    public virtual void UpdateRange(IEnumerable<T> entities) => DbSet.UpdateRange(entities);

    public virtual void Remove(T entity) => DbSet.Remove(entity);

    public virtual void RemoveRange(IEnumerable<T> entities) => DbSet.RemoveRange(entities);

    // ---- Read ------------------------------------------------------------------------------------------------

    public virtual async Task<T?> FindAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull =>
        await DbSet.FindAsync([id], cancellationToken);

    public virtual Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public virtual Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public virtual Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);

    public virtual Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken);

    public virtual Task<List<T>> ListAsync(CancellationToken cancellationToken = default) =>
        DbSet.ToListAsync(cancellationToken);

    public virtual async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var result = await ApplySpecification(specification).ToListAsync(cancellationToken);
        return specification.PostProcessingAction is null ? result : [.. specification.PostProcessingAction(result)];
    }

    public virtual async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var result = await ApplySpecification(specification).ToListAsync(cancellationToken);
        return specification.PostProcessingAction is null ? result : [.. specification.PostProcessingAction(result)];
    }

    public virtual async Task<PaginatedResult<T>> PageAsync(ISpecification<T> specification, IBaseFilter filter, CancellationToken cancellationToken = default)
    {
        var count = await ApplySpecification(specification, evaluateCriteriaOnly: true).CountAsync(cancellationToken);
        var pagination = new Pagination(count, filter);

        var data = await ApplySpecification(specification)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<T>(data, pagination);
    }

    public virtual async Task<PaginatedResult<TResult>> PageAsync<TResult>(ISpecification<T, TResult> specification, IBaseFilter filter, CancellationToken cancellationToken = default)
    {
        var count = await ApplySpecification(specification, evaluateCriteriaOnly: true).CountAsync(cancellationToken);
        var pagination = new Pagination(count, filter);

        var data = await ApplySpecification(specification)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<TResult>(data, pagination);
    }

    public virtual Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification, evaluateCriteriaOnly: true).CountAsync(cancellationToken);

    public virtual Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        DbSet.CountAsync(cancellationToken);

    public virtual Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification, evaluateCriteriaOnly: true).AnyAsync(cancellationToken);

    public virtual Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        DbSet.AnyAsync(cancellationToken);

    public virtual IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification) =>
        ApplySpecification(specification).AsAsyncEnumerable();

    protected virtual IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false) =>
        Evaluator.GetQuery(DbSet, specification, evaluateCriteriaOnly);

    protected virtual IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> specification) =>
        Evaluator.GetQuery(DbSet, specification);
}
