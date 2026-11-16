using ShipFlow.Ordering.Contracts.Common;

namespace ShipFlow.Ordering.Contracts.Orders.Responses;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    EnumResponse Status,
    AddressResponse ShippingAddress,
    IReadOnlyList<OrderLineResponse> Lines,
    MoneyResponse Total,
    DateTimeOffset PlacedAt);

public sealed record OrderLineResponse(Guid Id, Guid ProductId, int Quantity, MoneyResponse UnitPrice, MoneyResponse LineTotal);

public sealed record AddressResponse(string Street, string City, string PostalCode, string CountryCode);

public sealed record MoneyResponse(decimal Amount, string Currency);
