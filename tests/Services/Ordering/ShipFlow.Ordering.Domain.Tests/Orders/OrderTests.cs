using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Domain.Application.Orders.Enums;
using ShipFlow.Ordering.Domain.Application.Orders.Events;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.Errors;

namespace ShipFlow.Ordering.Domain.Tests.Orders;

/// <summary>
/// Test cases 1–4 of docs/specs/001-ordering-place-order.md.
/// </summary>
public class OrderTests
{
    [Fact]
    public void Place_ValidLines_IsPlacedWithCorrectTotalAndRaisesOrderPlaced()
    {
        var customer = OrderBuilder.Customer();

        var result = Order.Place(
            customer,
            OrderBuilder.ShippingAddress(),
            [OrderBuilder.Line(quantity: 2, unitPrice: 19.99m), OrderBuilder.Line(quantity: 1, unitPrice: 5m)],
            OrderBuilder.PlacedAt);

        result.IsError.ShouldBeFalse();
        var order = result.Value;
        order.Status.ShouldBe(OrderStatus.Placed);
        order.Lines.Count.ShouldBe(2);
        order.Total.Amount.ShouldBe(44.98m);
        order.Total.Currency.ShouldBe("EUR");
        order.PlacedAt.ShouldBe(OrderBuilder.PlacedAt);

        var placed = order.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<OrderPlacedDomainEvent>();
        placed.OrderId.ShouldBe(order.Id.Value);
        placed.CustomerId.ShouldBe(customer.Value);
        placed.TotalAmount.ShouldBe(44.98m);
    }

    [Fact]
    public void Place_NoLines_ReturnsLinesRequired()
    {
        var result = Order.Place(OrderBuilder.Customer(), OrderBuilder.ShippingAddress(), [], OrderBuilder.PlacedAt);

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(error => error.Code == DomainErrors.Order.LinesRequired.Code);
    }

    [Fact]
    public void Place_MixedCurrencies_ReturnsMixedCurrencies()
    {
        var result = Order.Place(
            OrderBuilder.Customer(),
            OrderBuilder.ShippingAddress(),
            [OrderBuilder.Line(currency: "EUR"), OrderBuilder.Line(currency: "USD")],
            OrderBuilder.PlacedAt);

        result.IsError.ShouldBeTrue();
        result.Errors.ShouldContain(error => error.Code == DomainErrors.Order.MixedCurrencies.Code);
    }

    [Fact]
    public void Place_DuplicateProduct_MergesIntoOneLineWithSummedQuantity()
    {
        var product = ProductId.CreateUnique();

        var result = Order.Place(
            OrderBuilder.Customer(),
            OrderBuilder.ShippingAddress(),
            [OrderBuilder.Line(product, quantity: 2), OrderBuilder.Line(product, quantity: 3)],
            OrderBuilder.PlacedAt);

        var line = result.Value.Lines.ShouldHaveSingleItem();
        line.ProductId.ShouldBe(product);
        line.Quantity.Value.ShouldBe(5);
        result.Value.Total.Amount.ShouldBe(50m);
    }

    [Fact]
    public void Place_DuplicateProductWithDifferentPrices_ReturnsConflictingUnitPrice()
    {
        var product = ProductId.CreateUnique();

        var result = Order.Place(
            OrderBuilder.Customer(),
            OrderBuilder.ShippingAddress(),
            [OrderBuilder.Line(product, unitPrice: 10m), OrderBuilder.Line(product, unitPrice: 12m)],
            OrderBuilder.PlacedAt);

        result.Errors.ShouldContain(error => error.Code == DomainErrors.Order.ConflictingUnitPrice(product).Code);
    }

    [Fact]
    public void Cancel_PlacedOrder_SetsCancelled_SecondCancelFails()
    {
        var order = Order.Place(OrderBuilder.Customer(), OrderBuilder.ShippingAddress(), [OrderBuilder.Line()], OrderBuilder.PlacedAt).Value;

        order.Cancel().IsError.ShouldBeFalse();
        order.Status.ShouldBe(OrderStatus.Cancelled);

        order.Cancel().IsError.ShouldBeTrue();
    }
}
