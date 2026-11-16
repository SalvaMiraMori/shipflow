using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Domain.Tests.Orders;

internal static class OrderBuilder
{
    public static readonly DateTimeOffset PlacedAt = new(2026, 11, 12, 10, 0, 0, TimeSpan.Zero);

    public static CustomerId Customer() => CustomerId.CreateUnique();

    public static Address ShippingAddress() =>
        Address.CreateNew("Carrer de Balmes 1", "Barcelona", "08007", "ES").Value;

    public static NewOrderLine Line(ProductId? productId = null, int quantity = 1, decimal unitPrice = 10m, string currency = "EUR") =>
        new(productId ?? ProductId.CreateUnique(), Quantity.CreateNew(quantity).Value, Money.CreateNew(unitPrice, currency).Value);
}
