using FluentValidation;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.Validators;

public sealed class QuantityValidator : AbstractValidator<int>
{
    public static readonly QuantityValidator Instance = new();

    public QuantityValidator()
    {
        RuleFor(quantity => quantity)
            .InclusiveBetween(Quantity.MinValue, Quantity.MaxValue)
            .WithErrorCode($"{nameof(Quantity)}.OutOfRange")
            .WithMessage($"Quantity must be between {Quantity.MinValue} and {Quantity.MaxValue}.");
    }
}
