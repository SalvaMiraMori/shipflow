using Core.Application.Messaging;
using Core.Application.Repositories;
using ErrorOr;

namespace Core.Application.Behaviors;

/// <summary>
/// Commits the unit of work when the decorated command succeeds. Errors are never committed.
/// </summary>
public sealed class UnitOfWorkCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> inner,
    IUnitOfWork unitOfWork) : ICommandHandler<TCommand, TResult>
{
    public async Task<ErrorOr<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        var result = await inner.HandleAsync(command, cancellationToken);

        if (!result.IsError)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
