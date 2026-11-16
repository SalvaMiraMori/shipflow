using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using ShipFlow.Ordering.Contracts.Common;
using ShipFlow.Ordering.Contracts.Orders.Requests;
using ShipFlow.Ordering.Contracts.Orders.Responses;

namespace ShipFlow.Ordering.Api.IntegrationTests;

/// <summary>
/// Test cases 5–6 of docs/specs/001-ordering-place-order.md, through HTTP and a real database.
/// </summary>
public class OrdersEndpointsTests(OrderingApiFactory factory) : IClassFixture<OrderingApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static PlaceOrderRequest ValidRequest(Guid? productId = null)
    {
        var product = productId ?? Guid.NewGuid();
        return new PlaceOrderRequest(
            Guid.NewGuid(),
            new AddressRequest("Carrer de Balmes 1", "Barcelona", "08007", "ES"),
            [
                new OrderLineRequest(product, 2, 19.99m, "EUR"),
                new OrderLineRequest(product, 1, 19.99m, "EUR"),
            ]);
    }

    [Fact]
    public async Task PostThenGet_ValidOrder_RoundTripsWithMergedLines()
    {
        var ct = TestContext.Current.CancellationToken;
        var request = ValidRequest();

        var post = await _client.PostAsJsonAsync("/api/orders", request, ct);

        post.StatusCode.ShouldBe(HttpStatusCode.Created);
        post.Headers.Location.ShouldNotBeNull();
        var created = await post.Content.ReadFromJsonAsync<OrderResponse>(ct);
        created.ShouldNotBeNull();

        var fetched = await _client.GetFromJsonAsync<OrderResponse>(post.Headers.Location, ct);

        fetched.ShouldNotBeNull();
        fetched.Id.ShouldBe(created.Id);
        fetched.CustomerId.ShouldBe(request.CustomerId);
        fetched.Status.Name.ShouldBe("Placed");
        fetched.Lines.ShouldHaveSingleItem().Quantity.ShouldBe(3);
        fetched.Total.ShouldBe(new MoneyResponse(59.97m, "EUR"));
    }

    [Fact]
    public async Task Post_InvalidPayload_Returns400ProblemDetailsWithFieldErrors()
    {
        var ct = TestContext.Current.CancellationToken;
        var request = new PlaceOrderRequest(Guid.NewGuid(), new AddressRequest("Street 1", "City", "08001", "ES"), []);

        var response = await _client.PostAsJsonAsync("/api/orders", request, ct);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(ct);
        problem.ShouldNotBeNull();
        problem.Errors.Keys.ShouldContain("Order.Lines");
    }

    [Fact]
    public async Task Get_UnknownOrder_Returns404()
    {
        var response = await _client.GetAsync($"/api/orders/{Guid.NewGuid()}", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_AfterPlacingAnOrder_ReturnsPagedResult()
    {
        var ct = TestContext.Current.CancellationToken;
        var request = ValidRequest();
        (await _client.PostAsJsonAsync("/api/orders", request, ct)).EnsureSuccessStatusCode();

        var page = await _client.GetFromJsonAsync<PagedResponse<OrderSummaryResponse>>(
            $"/api/orders?customerId={request.CustomerId}&status=placed", ct);

        page.ShouldNotBeNull();
        page.TotalItems.ShouldBe(1);
        page.Data.ShouldHaveSingleItem().CustomerId.ShouldBe(request.CustomerId);
    }
}
