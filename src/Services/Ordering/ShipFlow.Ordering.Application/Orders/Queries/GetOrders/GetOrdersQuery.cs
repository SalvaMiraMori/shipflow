using Core.Domain.Common;
using ShipFlow.Ordering.Domain.Application.Orders.Filters;

namespace ShipFlow.Ordering.Application.Orders.Queries.GetOrders;

/// <summary>
/// Paged list of orders, optionally filtered by customer and status (EcoGreen's <c>GetCompaniesQueryRequest</c> pattern).
/// </summary>
public sealed record GetOrdersQuery(Guid? CustomerId, string? Status) : BaseFilter, IOrderFilter;
