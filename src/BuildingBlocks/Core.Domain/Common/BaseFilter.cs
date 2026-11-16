namespace Core.Domain.Common;

/// <summary>
/// Paging and sorting parameters. <see cref="OrderBy"/> accepts <c>asc</c> (default) or <c>desc</c>.
/// </summary>
public record BaseFilter : IBaseFilter
{
    public const string Descending = "desc";

    public int? Page { get; init; }

    public int? PageSize { get; init; }

    public string? SortBy { get; init; }

    public string? OrderBy { get; init; }

    public bool IsDescending => string.Equals(OrderBy, Descending, StringComparison.OrdinalIgnoreCase);
}
