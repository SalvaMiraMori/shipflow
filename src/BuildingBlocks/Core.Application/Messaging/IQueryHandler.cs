using ErrorOr;

namespace Core.Application.Messaging;

/// <summary>
/// Handles a query (a request that reads state and has no side effects).
/// </summary>
public interface IQueryHandler<in TQuery, TResult>
{
    Task<ErrorOr<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
