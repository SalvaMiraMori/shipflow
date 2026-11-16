namespace ShipFlow.Ordering.Contracts.Common;

/// <summary>
/// Paging and sorting query-string parameters shared by list endpoints.
/// </summary>
public record FilterParameters
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;

    public int Page { get; init; } = DefaultPage;

    public int PageSize { get; init; } = DefaultPageSize;

    public string? SortBy { get; init; }

    /// <summary><c>asc</c> or <c>desc</c>.</summary>
    public string? OrderBy { get; init; }
}
