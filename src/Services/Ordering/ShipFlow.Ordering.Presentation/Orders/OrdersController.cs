using Core.Application.Messaging;
using Core.Domain.Common;
using Core.Presentation.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Ordering.Application.Orders.Commands.CancelOrder;
using ShipFlow.Ordering.Application.Orders.Commands.PlaceOrder;
using ShipFlow.Ordering.Application.Orders.Queries.GetOrderById;
using ShipFlow.Ordering.Application.Orders.Queries.GetOrders;
using ShipFlow.Ordering.Contracts.Common;
using ShipFlow.Ordering.Contracts.Orders.Requests;
using ShipFlow.Ordering.Contracts.Orders.Responses;
using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Presentation.Contracts;
using ShipFlow.Ordering.Presentation.Orders.Mapping;

namespace ShipFlow.Ordering.Presentation.Orders;

/// <summary>
/// Orders API (spec 001). Thin: map request → command/query, call the handler, map result → response or Problem Details.
/// Handlers are injected per action with <c>[FromServices]</c> so each request only resolves what it uses
/// (EcoGreen injected every handler in the constructor).
/// </summary>
public sealed class OrdersController : ApiController
{
    [HttpPost(ApiRoutes.Orders.Place)]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrder(
        [FromBody] PlaceOrderRequest request,
        [FromServices] ICommandHandler<PlaceOrderCommand, Order> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request.ToCommand(), cancellationToken);

        return result.Match<IActionResult>(
            order => CreatedAtAction(nameof(GetOrderById), new { orderId = order.Id.Value }, order.ToResponse()),
            Problem);
    }

    [HttpGet(ApiRoutes.Orders.GetById, Name = nameof(GetOrderById))]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(
        Guid orderId,
        [FromServices] IQueryHandler<GetOrderByIdQuery, Order> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetOrderByIdQuery(orderId), cancellationToken);

        return result.Match<IActionResult>(order => Ok(order.ToResponse()), Problem);
    }

    [HttpGet(ApiRoutes.Orders.GetAll)]
    [ProducesResponseType<PagedResponse<OrderSummaryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrders(
        [FromQuery] GetOrdersRequest request,
        [FromServices] IQueryHandler<GetOrdersQuery, PaginatedResult<Order>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(request.ToQuery(), cancellationToken);

        // An empty page is still a 200 with an empty list (EcoGreen returned 204, which clients handle poorly).
        return result.Match<IActionResult>(page => Ok(page.ToPagedResponse(OrderMappings.ToSummaryResponse)), Problem);
    }

    [HttpPost(ApiRoutes.Orders.Cancel)]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelOrder(
        Guid orderId,
        [FromServices] ICommandHandler<CancelOrderCommand, Order> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new CancelOrderCommand(orderId), cancellationToken);

        return result.Match<IActionResult>(order => Ok(order.ToResponse()), Problem);
    }
}
