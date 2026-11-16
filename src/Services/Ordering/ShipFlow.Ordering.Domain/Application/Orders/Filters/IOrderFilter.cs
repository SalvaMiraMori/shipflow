using Core.Domain.Common;

namespace ShipFlow.Ordering.Domain.Application.Orders.Filters;

public interface IOrderFilter : IBaseFilter
{
    Guid? CustomerId { get; }

    string? Status { get; }
}
