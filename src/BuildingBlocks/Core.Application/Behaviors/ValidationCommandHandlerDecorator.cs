using Core.Application.Messaging;
using Core.Domain.Validation;
using ErrorOr;
using FluentValidation;

namespace Core.Application.Behaviors;

/// <summary>
/// Runs every registered <see cref="IValidator{T}"/> for the command before the handler and short-circuits with
/// validation errors (rendered as HTTP 400 by the presentation layer). Input validation therefore lives next to the
/// command instead of inside each handler.
/// </summary>
public sealed class ValidationCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> inner,
    IEnumerable<IValidator<TCommand>> validators) : ICommandHandler<TCommand, TResult>
{
    public async Task<ErrorOr<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(command, cancellationToken);
            if (!result.IsValid)
            {
                errors.AddRange(result.ToErrors());
            }
        }

        return errors.Count > 0 ? errors : await inner.HandleAsync(command, cancellationToken);
    }
}
