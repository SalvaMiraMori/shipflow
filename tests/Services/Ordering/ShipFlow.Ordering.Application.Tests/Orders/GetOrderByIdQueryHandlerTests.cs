using Core.Application.Repositories;
using Core.Specification;
using ErrorOr;
using NSubstitute;
using ShipFlow.Ordering.Application.Orders.Queries.GetOrderById;
using ShipFlow.Ordering.Domain.Application.Orders;

namespace ShipFlow.Ordering.Application.Tests.Orders;

public class GetOrderByIdQueryHandlerTests
{
    private readonly IReadRepository<Order> _repository = Substitute.For<IReadRepository<Order>>();

    [Fact]
    public async Task HandleAsync_UnknownOrder_ReturnsNotFound()
    {
        _repository.SingleOrDefaultAsync(Arg.Any<ISingleResultSpecification<Order>>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);
        var handler = new GetOrderByIdQueryHandler(_repository);

        var result = await handler.HandleAsync(new GetOrderByIdQuery(Guid.NewGuid()), TestContext.Current.CancellationToken);

        result.FirstError.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task HandleAsync_EmptyId_ReturnsValidationErrorWithoutQuerying()
    {
        var handler = new GetOrderByIdQueryHandler(_repository);

        var result = await handler.HandleAsync(new GetOrderByIdQuery(Guid.Empty), TestContext.Current.CancellationToken);

        result.FirstError.Type.ShouldBe(ErrorType.Validation);
        await _repository.DidNotReceiveWithAnyArgs().SingleOrDefaultAsync(default(ISingleResultSpecification<Order>)!, Arg.Any<CancellationToken>());
    }
}
