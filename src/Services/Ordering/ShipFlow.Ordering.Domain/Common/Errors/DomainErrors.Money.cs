using ErrorOr;

namespace ShipFlow.Ordering.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class Money
    {
        public static Error CurrencyMismatch(string left, string right) =>
            Error.Validation($"{nameof(Money)}.CurrencyMismatch", $"Cannot combine amounts in {left} and {right}.");
    }
}
