using Core.Application.Repositories;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using ShipFlow.Ordering.Application.Orders.Commands.PlaceOrder;
using ShipFlow.Ordering.Domain.Application.Orders;

namespace ShipFlow.Ordering.Application.Tests.Orders;

public class PlaceOrderCommandHandlerTests
{
    private static readonly DateTimeOffset _now = new(2026, 11, 12, 10, 0, 0, TimeSpan.Zero);

    private readonly IRepository<Order> _repository = Substitute.For<IRepository<Order>>();
    private readonly PlaceOrderCommandHandler _handler;

    public PlaceOrderCommandHandlerTests()
    {
        _handler = new PlaceOrderCommandHandler(_repository, new FakeTimeProvider(_now));
    }

    private static PlaceOrderCommand ValidCommand(params PlaceOrderCommand.LineInput[] lines) => new(
        Guid.NewGuid(),
        new PlaceOrderCommand.AddressInput("Carrer de Balmes 1", "Barcelona", "08007", "ES"),
        lines.Length > 0 ? lines : [new PlaceOrderCommand.LineInput(Guid.NewGuid(), 2, 10m, "EUR")]);

    [Fact]
    public async Task HandleAsync_ValidCommand_AddsOrderStampedWithCurrentTime()
    {
        var result = await _handler.HandleAsync(ValidCommand(), TestContext.Current.CancellationToken);

        result.IsError.ShouldBeFalse();
        result.Value.PlacedAt.ShouldBe(_now);
        result.Value.Total.Amount.ShouldBe(20m);
        await _repository.Received(1).AddAsync(result.Value, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_SeveralInvalidFields_ReturnsAllErrorsAndAddsNothing()
    {
        var command = new PlaceOrderCommand(
            Guid.Empty,
            new PlaceOrderCommand.AddressInput("", "Barcelona", "08007", "Spain"),
            [new PlaceOrderCommand.LineInput(Guid.NewGuid(), 0, -1m, "EUR")]);

        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        result.IsError.ShouldBeTrue();
        // Customer id, street, country code, quantity and unit price: every problem is reported at once.
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(5);
        await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_MixedCurrencies_ReturnsDomainErrorAndAddsNothing()
    {
        var command = ValidCommand(
            new PlaceOrderCommand.LineInput(Guid.NewGuid(), 1, 10m, "EUR"),
            new PlaceOrderCommand.LineInput(Guid.NewGuid(), 1, 10m, "USD"));

        var result = await _handler.HandleAsync(command, TestContext.Current.CancellationToken);

        result.FirstError.Code.ShouldBe("Order.Lines.MixedCurrencies");
        await _repository.DidNotReceiveWithAnyArgs().AddAsync(default!, Arg.Any<CancellationToken>());
    }
}
