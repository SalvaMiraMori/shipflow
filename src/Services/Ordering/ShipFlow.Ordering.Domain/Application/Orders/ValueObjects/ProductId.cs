using Core.Domain.Abstractions;
using Core.Domain.Validation;
using ErrorOr;
using ShipFlow.Ordering.Domain.Application.Orders.Validators;

namespace ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;

/// <summary>
/// Reference to a catalogue product (owned by the Inventory service).
/// </summary>
public sealed class ProductId : EntityId<Guid>
{
    private ProductId(Guid value) => Value = value;

    private ProductId()
    {
    }

    public override Guid Value { get; protected set; }

    public static ProductId CreateUnique() => new(Guid.NewGuid());

    /// <summary>
    /// Creates the id from an existing value; <see cref="Guid.Empty"/> is rejected.
    /// </summary>
    public static ErrorOr<ProductId> Create(Guid value) => ProductIdValidator.Instance.ValidateAndCreate(value, v => new ProductId(v));

    public static implicit operator Guid(ProductId id) => id.Value;
}
