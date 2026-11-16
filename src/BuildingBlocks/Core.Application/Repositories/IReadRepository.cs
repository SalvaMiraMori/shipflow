using Core.Domain.Common;
using Core.Specification;

namespace Core.Application.Repositories;

/// <summary>
/// Read operations driven by specifications. Queries are expressed as <see cref="ISpecification{T}"/> objects
/// in the Domain layer, so handlers never see <see cref="IQueryable{T}"/> or EF Core.
/// </summary>
/// <remarks>
/// Replaces the four overlapping read interfaces of the previous Core. Projections use
/// <see cref="ISpecification{T, TResult}"/> (a <c>Select</c> in the spec) instead of a mapper inside persistence,
/// and nothing swallows exceptions.
/// </remarks>
public interface IReadRepository<T>
    where T : class
{
    /// <summary>Finds a tracked entity by primary key (also accepts strongly-typed ids).</summary>
    Task<T?> FindAsync<TId>(TId id, CancellationToken cancellationToken = default)
        where TId : notnull;

    Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default);

    Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);

    Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default);

    /// <summary>Counts and fetches one page. Paging comes from <paramref name="filter"/>, not from the specification.</summary>
    Task<PaginatedResult<T>> PageAsync(ISpecification<T> specification, IBaseFilter filter, CancellationToken cancellationToken = default);

    Task<PaginatedResult<TResult>> PageAsync<TResult>(ISpecification<T, TResult> specification, IBaseFilter filter, CancellationToken cancellationToken = default);

    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    IAsyncEnumerable<T> AsAsyncEnumerable(ISpecification<T> specification);
}
