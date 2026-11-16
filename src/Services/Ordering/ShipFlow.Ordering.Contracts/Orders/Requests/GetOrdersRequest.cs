using ShipFlow.Ordering.Contracts.Common;

namespace ShipFlow.Ordering.Contracts.Orders.Requests;

public sealed record GetOrdersRequest : FilterParameters
{
    public Guid? CustomerId { get; init; }

    public string? Status { get; init; }
}
