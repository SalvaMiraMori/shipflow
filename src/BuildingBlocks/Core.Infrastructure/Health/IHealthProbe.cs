namespace Core.Infrastructure.Health;

/// <summary>
/// Implemented by clients of external dependencies that can report their own health.
/// </summary>
public interface IHealthProbe
{
    Task<string> GetHealthAsync(CancellationToken cancellationToken);
}
