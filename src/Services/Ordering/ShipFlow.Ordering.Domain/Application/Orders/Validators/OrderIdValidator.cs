using Core.Domain.Validation.Extensions;
using FluentValidation;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Validators;

public sealed class OrderIdValidator : AbstractValidator<Guid>
{
    public static readonly OrderIdValidator Instance = new();

    public OrderIdValidator()
    {
        this.AddIdValidation(id => id, nameof(OrderId));
    }
}
