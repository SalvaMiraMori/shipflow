using FluentValidation;

namespace ShipFlow.Ordering.Application.Orders.Commands.PlaceOrder;

/// <summary>
/// Shape checks only (required parts present). Business rules live in the domain, so they hold for every caller.
/// Runs automatically through <c>ValidationCommandHandlerDecorator</c>.
/// </summary>
public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(command => command.ShippingAddress)
            .NotNull()
            .WithErrorCode("Order.ShippingAddress.Required")
            .WithMessage("The shipping address is required.");

        RuleFor(command => command.Lines)
            .NotEmpty()
            .WithErrorCode("Order.Lines.Empty")
            .WithMessage("An order needs at least one line.");
    }
}
