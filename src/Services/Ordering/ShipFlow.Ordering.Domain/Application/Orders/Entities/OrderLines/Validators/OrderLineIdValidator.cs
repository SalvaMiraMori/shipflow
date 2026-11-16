using Core.Domain.Validation.Extensions;
using FluentValidation;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.Validators;

public sealed class OrderLineIdValidator : AbstractValidator<Guid>
{
    public static readonly OrderLineIdValidator Instance = new();

    public OrderLineIdValidator()
    {
        this.AddIdValidation(id => id, nameof(OrderLineId));
    }
}
