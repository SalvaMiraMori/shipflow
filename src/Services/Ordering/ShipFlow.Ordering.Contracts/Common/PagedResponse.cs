namespace ShipFlow.Ordering.Contracts.Common;

/// <summary>
/// A page of results. Fixes EcoGreen's version: no fake <c>async</c> factory, immutable, and total pages /
/// next-page flags come from the server-side pagination instead of being recomputed (which divided by zero
/// when the page size was 0).
/// </summary>
public sealed record PagedResponse<T>(
    IReadOnlyList<T> Data,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
