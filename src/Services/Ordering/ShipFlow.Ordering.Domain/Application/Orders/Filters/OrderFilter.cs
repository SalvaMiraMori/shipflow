using Core.Domain.Common;

namespace ShipFlow.Ordering.Domain.Application.Orders.Filters;

/// <summary>
/// Filtering, sorting and paging options for listing orders.
/// </summary>
public sealed record OrderFilter : BaseFilter, IOrderFilter
{
    public Guid? CustomerId { get; init; }

    public string? Status { get; init; }

    public static OrderFilter FromQuery(IOrderFilter query) => new()
    {
        CustomerId = query.CustomerId,
        Status = query.Status,
        Page = query.Page,
        PageSize = query.PageSize,
        SortBy = query.SortBy,
        OrderBy = query.OrderBy,
    };
}
