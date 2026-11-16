using Core.Domain.Errors;
using ErrorOr;

namespace ShipFlow.Ordering.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class Order
    {
        private const string Entity = nameof(Order);

        public static Error NotFound(Guid id) => ErrorTemplates.NotFound(Entity, id);

        public static Error LinesRequired => Error.Validation($"{Entity}.Lines.Empty", "An order needs at least one line.");

        public static Error MixedCurrencies => Error.Validation($"{Entity}.Lines.MixedCurrencies", "All order lines must use the same currency.");

        public static Error ConflictingUnitPrice(Guid productId) =>
            Error.Validation($"{Entity}.Lines.ConflictingUnitPrice", $"Product {productId} appears more than once with different unit prices.");

        public static Error InvalidStatusTransition(string from, string to) =>
            ErrorTemplates.InvalidOperation(Entity, $"cannot change status from {from} to {to}.");
    }
}
