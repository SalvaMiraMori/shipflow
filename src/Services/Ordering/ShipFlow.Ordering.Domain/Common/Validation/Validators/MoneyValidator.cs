using Core.Domain.Validation.Extensions;
using FluentValidation;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Domain.Common.Validation.Validators;

public sealed class MoneyValidator : AbstractValidator<Money>
{
    public static readonly MoneyValidator Instance = new();

    public MoneyValidator()
    {
        this.AddMinValueValidation(money => money.Amount, Money.MinAmount, nameof(Money.Amount));
        this.AddRegexValidation(money => money.Currency, Money.CurrencyPattern, nameof(Money.Currency));
    }
}
