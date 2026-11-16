namespace ShipFlow.Ordering.Contracts.Orders.Requests;

public sealed record PlaceOrderRequest(Guid CustomerId, AddressRequest ShippingAddress, IReadOnlyList<OrderLineRequest> Lines);

public sealed record AddressRequest(string Street, string City, string PostalCode, string CountryCode);

public sealed record OrderLineRequest(Guid ProductId, int Quantity, decimal UnitPrice, string Currency);
