namespace Core.Domain.Common;

/// <summary>
/// A page of items plus its pagination details.
/// </summary>
public sealed record PaginatedResult<T>(IReadOnlyList<T> Data, Pagination Pagination)
{
    public PaginatedResult<TOut> Map<TOut>(Func<T, TOut> selector) => new(Data.Select(selector).ToList(), Pagination);
}
