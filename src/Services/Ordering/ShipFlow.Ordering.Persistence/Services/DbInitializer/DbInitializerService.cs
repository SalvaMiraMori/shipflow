using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShipFlow.Ordering.Persistence.Data;

namespace ShipFlow.Ordering.Persistence.Services.DbInitializer;

/// <summary>
/// Applies pending migrations on start-up (EcoGreen pattern). Until the first migration exists
/// (<c>dotnet ef migrations add InitialCreate</c>), it creates the schema from the model so the API runs out of the box.
/// In Azure, migrations run as a pipeline step instead (see docs/adr/0002).
/// </summary>
internal sealed class DbInitializerService(OrderingDbContext dbContext, ILogger<DbInitializerService> logger) : IDbInitializerService
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (dbContext.Database.GetMigrations().Any())
        {
            logger.LogInformation("Applying Ordering database migrations");
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            logger.LogWarning("No migrations found: creating the Ordering schema from the model (development only)");
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}
