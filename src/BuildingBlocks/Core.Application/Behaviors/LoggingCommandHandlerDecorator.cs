using System.Diagnostics;
using Core.Application.Messaging;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Core.Application.Behaviors;

/// <summary>
/// Structured log line per command: name, outcome, error codes and elapsed time.
/// </summary>
public sealed class LoggingCommandHandlerDecorator<TCommand, TResult>(
    ICommandHandler<TCommand, TResult> inner,
    ILogger<LoggingCommandHandlerDecorator<TCommand, TResult>> logger) : ICommandHandler<TCommand, TResult>
{
    public async Task<ErrorOr<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;
        var startedAt = Stopwatch.GetTimestamp();

        var result = await inner.HandleAsync(command, cancellationToken);
        var elapsedMs = Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds;

        if (result.IsError)
        {
            logger.LogWarning(
                "Command {Command} failed with {ErrorCodes} in {ElapsedMs:0.0} ms",
                commandName,
                string.Join(", ", result.Errors.Select(error => error.Code)),
                elapsedMs);
        }
        else
        {
            logger.LogInformation("Command {Command} handled in {ElapsedMs:0.0} ms", commandName, elapsedMs);
        }

        return result;
    }
}
