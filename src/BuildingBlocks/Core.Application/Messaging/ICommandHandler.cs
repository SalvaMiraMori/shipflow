using ErrorOr;

namespace Core.Application.Messaging;

/// <summary>
/// Handles a command (a request that changes state). Successful commands are committed by
/// <see cref="Behaviors.UnitOfWorkCommandHandlerDecorator{TCommand, TResult}"/>; handlers never call SaveChanges.
/// </summary>
/// <remarks>
/// Same shape as EcoGreen's handlers, promoted to Core so every service shares it. No MediatR:
/// controllers inject the handler they need, which keeps dependencies explicit and avoids MediatR's commercial licence.
/// </remarks>
public interface ICommandHandler<in TCommand, TResult>
{
    Task<ErrorOr<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
