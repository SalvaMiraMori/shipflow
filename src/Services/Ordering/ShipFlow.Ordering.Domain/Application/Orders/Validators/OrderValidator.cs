using FluentValidation;
using ShipFlow.Ordering.Domain.Common.Errors;

namespace ShipFlow.Ordering.Domain.Application.Orders.Validators;

/// <summary>
/// Aggregate-level invariants of <see cref="Order"/>, checked after every state change (EcoGreen pattern).
/// </summary>
public sealed class OrderValidator : AbstractValidator<Order>
{
    public static readonly OrderValidator Instance = new();

    public OrderValidator()
    {
        RuleFor(order => order.Lines)
            .NotEmpty()
            .WithErrorCode(DomainErrors.Order.LinesRequired.Code)
            .WithMessage(DomainErrors.Order.LinesRequired.Description);

        RuleFor(order => order.Lines)
            .Must(lines => lines.Select(line => line.UnitPrice.Currency).Distinct().Count() <= 1)
            .WithErrorCode(DomainErrors.Order.MixedCurrencies.Code)
            .WithMessage(DomainErrors.Order.MixedCurrencies.Description);
    }
}
