using Core.Domain.Common;
using ShipFlow.Ordering.Application.Orders.Commands.PlaceOrder;
using ShipFlow.Ordering.Application.Orders.Queries.GetOrders;
using ShipFlow.Ordering.Contracts.Common;
using ShipFlow.Ordering.Contracts.Orders.Requests;
using ShipFlow.Ordering.Contracts.Orders.Responses;
using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Presentation.Orders.Mapping;

/// <summary>
/// Contract ↔ application mappings, one class per feature folder (EcoGreen's <c>Mapping/*MappingConfig</c>).
/// Plain extension methods instead of Mapster: a renamed property is a compile error instead of a runtime surprise,
/// and value objects (<c>OrderId</c> → <c>Guid</c>) need no custom converters.
/// </summary>
public static class OrderMappings
{
    // ---- Requests ---------------------------------------------------------------------------------------------

    public static PlaceOrderCommand ToCommand(this PlaceOrderRequest request) => new(
        request.CustomerId,
        new PlaceOrderCommand.AddressInput(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.PostalCode,
            request.ShippingAddress.CountryCode),
        [.. request.Lines.Select(line => new PlaceOrderCommand.LineInput(line.ProductId, line.Quantity, line.UnitPrice, line.Currency))]);

    public static GetOrdersQuery ToQuery(this GetOrdersRequest request) => new(request.CustomerId, request.Status)
    {
        Page = request.Page,
        PageSize = request.PageSize,
        SortBy = request.SortBy,
        OrderBy = request.OrderBy,
    };

    // ---- Responses --------------------------------------------------------------------------------------------

    public static OrderResponse ToResponse(this Order order) => new(
        order.Id,
        order.CustomerId,
        new EnumResponse(order.Status.Value, order.Status.Name),
        new AddressResponse(order.ShippingAddress.Street, order.ShippingAddress.City, order.ShippingAddress.PostalCode, order.ShippingAddress.CountryCode),
        [.. order.Lines.Select(ToResponse)],
        order.Total.ToResponse(),
        order.PlacedAt);

    public static OrderSummaryResponse ToSummaryResponse(this Order order) => new(
        order.Id,
        order.CustomerId,
        new EnumResponse(order.Status.Value, order.Status.Name),
        order.Lines.Count,
        order.Total.ToResponse(),
        order.PlacedAt);

    public static OrderLineResponse ToResponse(this OrderLine line) => new(
        line.Id,
        line.ProductId,
        line.Quantity,
        line.UnitPrice.ToResponse(),
        line.LineTotal.ToResponse());

    public static MoneyResponse ToResponse(this Money money) => new(money.Amount, money.Currency);

    public static PagedResponse<TResponse> ToPagedResponse<T, TResponse>(this PaginatedResult<T> page, Func<T, TResponse> map) => new(
        [.. page.Data.Select(map)],
        page.Pagination.Page,
        page.Pagination.PageSize,
        page.Pagination.TotalItems,
        page.Pagination.TotalPages,
        page.Pagination.HasPrevious,
        page.Pagination.HasNext);
}
