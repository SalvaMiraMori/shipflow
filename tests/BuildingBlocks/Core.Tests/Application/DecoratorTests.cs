using Core.Application.Behaviors;
using Core.Application.Messaging;
using Core.Application.Repositories;
using ErrorOr;
using FluentValidation;

namespace Core.Tests.Application;

public class DecoratorTests
{
    private sealed record Ping(string Value);

    private sealed class PingHandler(ErrorOr<string> result) : ICommandHandler<Ping, string>
    {
        public int Calls { get; private set; }

        public Task<ErrorOr<string>> HandleAsync(Ping command, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(result);
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int Saves { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            Saves++;
            return Task.FromResult(1);
        }

        public Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class PingValidator : AbstractValidator<Ping>
    {
        public PingValidator() => RuleFor(ping => ping.Value).NotEmpty().WithErrorCode("Ping.Value.Empty");
    }

    [Fact]
    public async Task UnitOfWork_SuccessfulCommand_SavesOnce()
    {
        var unitOfWork = new FakeUnitOfWork();
        var decorator = new UnitOfWorkCommandHandlerDecorator<Ping, string>(new PingHandler("pong"), unitOfWork);

        var result = await decorator.HandleAsync(new Ping("x"), TestContext.Current.CancellationToken);

        result.Value.ShouldBe("pong");
        unitOfWork.Saves.ShouldBe(1);
    }

    [Fact]
    public async Task UnitOfWork_FailedCommand_DoesNotSave()
    {
        var unitOfWork = new FakeUnitOfWork();
        var decorator = new UnitOfWorkCommandHandlerDecorator<Ping, string>(new PingHandler(Error.Conflict()), unitOfWork);

        var result = await decorator.HandleAsync(new Ping("x"), TestContext.Current.CancellationToken);

        result.IsError.ShouldBeTrue();
        unitOfWork.Saves.ShouldBe(0);
    }

    [Fact]
    public async Task Validation_InvalidCommand_ShortCircuitsBeforeHandler()
    {
        var handler = new PingHandler("pong");
        var decorator = new ValidationCommandHandlerDecorator<Ping, string>(handler, [new PingValidator()]);

        var result = await decorator.HandleAsync(new Ping(""), TestContext.Current.CancellationToken);

        result.FirstError.Code.ShouldBe("Ping.Value.Empty");
        result.FirstError.Type.ShouldBe(ErrorType.Validation);
        handler.Calls.ShouldBe(0);
    }
}
