namespace ShipFlow.Ordering.Application.Orders.Commands.PlaceOrder;

/// <summary>
/// Places an order. Primitive input; the handler turns it into value objects and the aggregate enforces the rules.
/// </summary>
public sealed record PlaceOrderCommand(
    Guid CustomerId,
    PlaceOrderCommand.AddressInput ShippingAddress,
    IReadOnlyList<PlaceOrderCommand.LineInput> Lines)
{
    public sealed record AddressInput(string Street, string City, string PostalCode, string CountryCode);

    public sealed record LineInput(Guid ProductId, int Quantity, decimal UnitPrice, string Currency);
}
