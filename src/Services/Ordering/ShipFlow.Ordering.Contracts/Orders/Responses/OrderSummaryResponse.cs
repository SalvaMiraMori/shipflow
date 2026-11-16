using ShipFlow.Ordering.Contracts.Common;

namespace ShipFlow.Ordering.Contracts.Orders.Responses;

public sealed record OrderSummaryResponse(Guid Id, Guid CustomerId, EnumResponse Status, int LineCount, MoneyResponse Total, DateTimeOffset PlacedAt);
