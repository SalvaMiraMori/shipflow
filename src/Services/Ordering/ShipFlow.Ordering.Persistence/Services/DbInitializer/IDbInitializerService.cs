namespace ShipFlow.Ordering.Persistence.Services.DbInitializer;

public interface IDbInitializerService
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
