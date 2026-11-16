using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Infrastructure.Health;

/// <summary>
/// Health check for an external dependency. Replaces <c>ApiHealthlyCheck</c>, which was decompiled code using
/// <c>dynamic</c> and ignored the cancellation token.
/// </summary>
public sealed class ExternalApiHealthCheck<TProbe>(TProbe probe) : IHealthCheck
    where TProbe : IHealthProbe
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var status = await probe.GetHealthAsync(cancellationToken);
            return HealthCheckResult.Healthy(data: new Dictionary<string, object> { ["status"] = status });
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "Dependency is not reachable.", exception);
        }
    }
}
