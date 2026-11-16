using Core.Domain.Abstractions;
using Core.Domain.Validation;
using ErrorOr;
using ShipFlow.Ordering.Domain.Common.Errors;
using ShipFlow.Ordering.Domain.Common.Validation.Validators;

namespace ShipFlow.Ordering.Domain.Common.ValueObjects;

/// <summary>
/// An amount in an ISO 4217 currency (e.g. <c>EUR</c>). Arithmetic across currencies is an error, never a silent bug.
/// </summary>
public sealed class Money : ValueObject
{
    public const decimal MinAmount = 0m;
    public const int CurrencyLength = 3;
    public const string CurrencyPattern = "^[A-Z]{3}$";

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

#pragma warning disable CS8618 // Required by EF Core.
    private Money()
    {
    }
#pragma warning restore CS8618

    public decimal Amount { get; private set; }

    public string Currency { get; private set; }

    /// <summary>
    /// Creates a validated amount. The currency is normalised to upper case.
    /// </summary>
    public static ErrorOr<Money> CreateNew(decimal amount, string? currency) =>
        MoneyValidator.Instance.ValidateToErrorOr(new Money(amount, currency?.Trim().ToUpperInvariant() ?? string.Empty));

    public static Money Zero(string currency) => new(0m, currency);

    /// <summary>
    /// Builds an amount from values that were already validated inside the domain (e.g. a sum of valid amounts).
    /// </summary>
    internal static Money FromTrusted(decimal amount, string currency) => new(amount, currency);

    public ErrorOr<Money> Add(Money other) =>
        other.Currency == Currency
            ? new Money(Amount + other.Amount, Currency)
            : DomainErrors.Money.CurrencyMismatch(Currency, other.Currency);

    public Money Multiply(int factor) => new(Amount * factor, Currency);

    public override string ToString() => $"{Amount:0.00} {Currency}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
