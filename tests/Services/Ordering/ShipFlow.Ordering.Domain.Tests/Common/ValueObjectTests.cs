using ShipFlow.Ordering.Domain.Application.Orders.Entities.OrderLines.ValueObjects;
using ShipFlow.Ordering.Domain.Application.Orders.ValueObjects;
using ShipFlow.Ordering.Domain.Common.ValueObjects;

namespace ShipFlow.Ordering.Domain.Tests.Common;

public class ValueObjectTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(Quantity.MaxValue + 1)]
    public void Quantity_OutOfRange_ReturnsError(int value)
    {
        Quantity.CreateNew(value).IsError.ShouldBeTrue();
    }

    [Fact]
    public void Money_NormalisesCurrencyToUpperCase()
    {
        var money = Money.CreateNew(10m, " eur ").Value;

        money.Currency.ShouldBe("EUR");
        money.ShouldBe(Money.CreateNew(10m, "EUR").Value);
    }

    [Theory]
    [InlineData(-0.01, "EUR")]
    [InlineData(1, "EURO")]
    [InlineData(1, "")]
    [InlineData(1, null)]
    public void Money_InvalidInput_ReturnsError(double amount, string? currency)
    {
        Money.CreateNew((decimal)amount, currency).IsError.ShouldBeTrue();
    }

    [Fact]
    public void Money_AddDifferentCurrencies_ReturnsError()
    {
        var euros = Money.CreateNew(1m, "EUR").Value;
        var dollars = Money.CreateNew(1m, "USD").Value;

        euros.Add(dollars).IsError.ShouldBeTrue();
    }

    [Fact]
    public void Address_InvalidCountryCode_ReturnsError()
    {
        Address.CreateNew("Street 1", "City", "08001", "Spain").IsError.ShouldBeTrue();
    }

    [Fact]
    public void Ids_EmptyGuid_ReturnsError()
    {
        OrderId.Create(Guid.Empty).IsError.ShouldBeTrue();
        CustomerId.Create(Guid.Empty).IsError.ShouldBeTrue();
        ProductId.Create(Guid.Empty).IsError.ShouldBeTrue();
    }
}
