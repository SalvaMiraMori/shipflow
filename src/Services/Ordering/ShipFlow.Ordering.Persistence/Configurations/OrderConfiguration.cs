using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines;
using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;
using ShipFlow.Ordering.Domain.Application.Orders.Enums;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.ValueObjects;
using ShipFlow.Ordering.Persistence.Constants;

namespace ShipFlow.Ordering.Persistence.Configurations;

/// <summary>
/// Maps the Order aggregate: strongly-typed ids and single-value objects through value converters, Address as an owned
/// type (columns on Orders), lines as an owned collection (own table, deleted with the order).
/// </summary>
public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    private const string RowVersion = nameof(RowVersion);

    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(TableNames.Orders);

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => OrderId.Create(value).Value);

        builder.Property(order => order.CustomerId)
            .HasConversion(id => id.Value, value => CustomerId.Create(value).Value)
            .IsRequired();
        builder.HasIndex(order => order.CustomerId);

        builder.Property(order => order.Status)
            .HasConversion(status => status.Value, value => OrderStatus.FromValue(value).Value)
            .IsRequired();

        builder.Property(order => order.PlacedAt).IsRequired();

        builder.OwnsOne(order => order.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(Address.StreetMaxLength).IsRequired();
            address.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(Address.CityMaxLength).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("ShippingPostalCode").HasMaxLength(Address.PostalCodeMaxLength).IsRequired();
            address.Property(a => a.CountryCode).HasColumnName("ShippingCountryCode").HasMaxLength(Address.CountryCodeLength).IsFixedLength().IsRequired();
        });
        builder.Navigation(order => order.ShippingAddress).IsRequired();

        builder.OwnsMany(order => order.Lines, ConfigureLines);
        builder.Navigation(order => order.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(order => order.Total);
        builder.Ignore(order => order.DomainEvents);

        // Optimistic concurrency: two requests changing the same order cannot silently overwrite each other.
        builder.Property<byte[]>(RowVersion).IsRowVersion();

        ConfigureAuditing(builder);
    }

    private static void ConfigureLines(OwnedNavigationBuilder<Order, OrderLine> line)
    {
        line.ToTable(TableNames.OrderLines);
        line.WithOwner().HasForeignKey("OrderId");

        line.HasKey(l => l.Id);
        line.Property(l => l.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => OrderLineId.Create(value).Value);

        line.Property(l => l.ProductId)
            .HasConversion(id => id.Value, value => ProductId.Create(value).Value)
            .IsRequired();

        line.Property(l => l.Quantity)
            .HasConversion(quantity => quantity.Value, value => Quantity.CreateNew(value).Value)
            .IsRequired();

        line.OwnsOne(l => l.UnitPrice, price =>
        {
            price.Property(p => p.Amount).HasColumnName("UnitPriceAmount").HasPrecision(18, 2).IsRequired();
            price.Property(p => p.Currency).HasColumnName("UnitPriceCurrency").HasMaxLength(Money.CurrencyLength).IsFixedLength().IsRequired();
        });
        line.Navigation(l => l.UnitPrice).IsRequired();

        line.Ignore(l => l.LineTotal);
        line.Ignore(l => l.DomainEvents);
    }

    private static void ConfigureAuditing(EntityTypeBuilder<Order> builder)
    {
        builder.Property(order => order.Created).IsRequired();
        builder.Property(order => order.CreatedBy).HasMaxLength(128);
        builder.Property(order => order.LastModified).IsRequired();
        builder.Property(order => order.LastModifiedBy).HasMaxLength(128);
        builder.Property(order => order.DeletedBy).HasMaxLength(128);
        builder.HasIndex(order => order.IsDeleted);
    }
}
