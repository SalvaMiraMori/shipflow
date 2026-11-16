using Core.Domain.Validation.Extensions;
using FluentValidation;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Validators;

public sealed class CustomerIdValidator : AbstractValidator<Guid>
{
    public static readonly CustomerIdValidator Instance = new();

    public CustomerIdValidator()
    {
        this.AddIdValidation(id => id, nameof(CustomerId));
    }
}
