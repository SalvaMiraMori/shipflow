namespace Core.Domain.Common;

/// <summary>
/// Paging and sorting parameters shared by list queries.
/// </summary>
public interface IBaseFilter
{
    int? Page { get; }

    int? PageSize { get; }

    string? SortBy { get; }

    string? OrderBy { get; }
}
