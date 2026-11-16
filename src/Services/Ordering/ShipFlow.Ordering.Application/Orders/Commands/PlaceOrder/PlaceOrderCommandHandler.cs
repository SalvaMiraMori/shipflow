using Core.Application.Messaging;
using Core.Application.Repositories;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Application.Orders.Commands.PlaceOrder;

/// <summary>
/// Builds the value objects (collecting every error, not just the first), places the order and adds it to the
/// repository. Committing is done by the UnitOfWork decorator.
/// </summary>
public sealed class PlaceOrderCommandHandler(IRepository<Order> orderRepository, TimeProvider timeProvider)
    : ICommandHandler<PlaceOrderCommand, Order>
{
    public async Task<ErrorOr<Order>> HandleAsync(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        var customerId = Collect(CustomerId.Create(command.CustomerId), errors);
        var address = Collect(
            Address.CreateNew(
                command.ShippingAddress.Street,
                command.ShippingAddress.City,
                command.ShippingAddress.PostalCode,
                command.ShippingAddress.CountryCode),
            errors);

        var lines = new List<NewOrderLine>();
        foreach (var line in command.Lines)
        {
            var productId = Collect(ProductId.Create(line.ProductId), errors);
            var quantity = Collect(Quantity.CreateNew(line.Quantity), errors);
            var unitPrice = Collect(Money.CreateNew(line.UnitPrice, line.Currency), errors);

            if (productId is not null && quantity is not null && unitPrice is not null)
            {
                lines.Add(new NewOrderLine(productId, quantity, unitPrice));
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        var order = Order.Place(customerId!, address!, lines, timeProvider.GetUtcNow());
        if (order.IsError)
        {
            return order.Errors;
        }

        await orderRepository.AddAsync(order.Value, cancellationToken);
        return order.Value;
    }

    private static T? Collect<T>(ErrorOr<T> result, List<Error> errors)
        where T : class
    {
        if (result.IsError)
        {
            errors.AddRange(result.Errors);
            return null;
        }

        return result.Value;
    }
}
