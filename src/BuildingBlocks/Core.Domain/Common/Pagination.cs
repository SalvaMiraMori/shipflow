using System.Text.Json.Serialization;

namespace Core.Domain.Common;

/// <summary>
/// Pagination details for a page of results. Out-of-range input is clamped instead of failing:
/// a page size above the limit becomes the limit, a page beyond the last becomes the last.
/// </summary>
public sealed class Pagination
{
    [JsonConstructor]
    public Pagination(int totalItems, int totalPages, int pageSize, int page)
    {
        TotalItems = totalItems;
        TotalPages = totalPages;
        PageSize = pageSize;
        Page = page;
    }

    public Pagination(int itemsCount, IBaseFilter filter, PaginationSettings? settings = null)
        : this(itemsCount, filter.PageSize, filter.Page, settings)
    {
    }

    public Pagination(int itemsCount, int? pageSize, int? page, PaginationSettings? settings = null)
    {
        settings ??= PaginationSettings.Default;

        TotalItems = Math.Max(itemsCount, 0);
        PageSize = pageSize is null or <= 0 ? settings.DefaultPageSize : Math.Min(pageSize.Value, settings.MaxPageSize);
        TotalPages = TotalItems == 0 ? 1 : (int)Math.Ceiling((decimal)TotalItems / PageSize);
        Page = page is null or <= 0 ? PaginationSettings.DefaultPage : Math.Min(page.Value, TotalPages);
    }

    public int TotalItems { get; }

    public int TotalPages { get; }

    public int PageSize { get; }

    public int Page { get; }

    public int StartItem => TotalItems == 0 ? 0 : (PageSize * (Page - 1)) + 1;

    public int EndItem => Math.Min(PageSize * Page, TotalItems);

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    [JsonIgnore]
    public int Skip => PageSize * (Page - 1);

    [JsonIgnore]
    public int Take => PageSize;
}
