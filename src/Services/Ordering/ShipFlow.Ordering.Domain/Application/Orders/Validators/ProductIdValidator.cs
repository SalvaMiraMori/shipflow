using Core.Domain.Validation.Extensions;
using FluentValidation;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

namespace ShipFlow.Ordering.Domain.Application.Orders.Validators;

public sealed class ProductIdValidator : AbstractValidator<Guid>
{
    public static readonly ProductIdValidator Instance = new();

    public ProductIdValidator()
    {
        this.AddIdValidation(id => id, nameof(ProductId));
    }
}
