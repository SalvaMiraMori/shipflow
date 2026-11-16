using Core.Domain.Common;
using Core.Specification.Builder;
using ShipFlow.Ordering.Domain.Application.Orders.Enums;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Specifications;

/// <summary>
/// Reusable query building blocks for <see cref="Order"/> specifications (EcoGreen pattern).
/// Note: compare whole value objects (<c>o.CustomerId.Equals(id)</c>), not <c>.Value</c>, so EF Core can apply the converter.
/// </summary>
public static class OrderSpecificationExtensions
{
    public const string SortByPlacedAt = "placedAt";
    public const string SortByStatus = "status";

    public static ISpecificationBuilder<Order> IdEquals(this ISpecificationBuilder<Order> builder, OrderId id) =>
        builder.Where(order => order.Id.Equals(id));

    public static ISpecificationBuilder<Order> CustomerIdEquals(this ISpecificationBuilder<Order> builder, CustomerId? customerId) =>
        builder.Where(order => order.CustomerId.Equals(customerId), customerId is not null);

    public static ISpecificationBuilder<Order> StatusEquals(this ISpecificationBuilder<Order> builder, OrderStatus? status) =>
        builder.Where(order => order.Status.Equals(status), status is not null);

    /// <summary>
    /// Sorts by <c>placedAt</c> (default, newest first) or <c>status</c>; <see cref="BaseFilter.OrderBy"/> = <c>asc</c>/<c>desc</c>.
    /// </summary>
    public static ISpecificationBuilder<Order> ApplyOrdering(this ISpecificationBuilder<Order> builder, BaseFilter? filter = null)
    {
        if (filter?.SortBy is null)
        {
            // Newest first unless the caller explicitly asks for ascending order.
            return string.Equals(filter?.OrderBy, "asc", StringComparison.OrdinalIgnoreCase)
                ? builder.OrderBy(order => order.PlacedAt)
                : builder.OrderByDescending(order => order.PlacedAt);
        }

        var descending = filter.IsDescending;
        return filter.SortBy.ToUpperInvariant() switch
        {
            "STATUS" => descending ? builder.OrderByDescending(order => order.Status) : builder.OrderBy(order => order.Status),
            _ => descending ? builder.OrderByDescending(order => order.PlacedAt) : builder.OrderBy(order => order.PlacedAt),
        };
    }
}
