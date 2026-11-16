using Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShipFlow.Ordering.Persistence.Data;
using ShipFlow.Ordering.Persistence.Repositories;
using ShipFlow.Ordering.Persistence.Services.DbInitializer;

namespace ShipFlow.Ordering.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// SQL Server (Azure SQL in the cloud). The connection string <c>ordering-db</c> is injected by Aspire locally and by
    /// Container Apps (Key Vault reference + Managed Identity) in Azure; nothing secret lives in appsettings.
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(OrderingDbContext.ConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{OrderingDbContext.ConnectionStringName}' is not configured.");

        services.AddDbContext<OrderingDbContext>(options => options.UseSqlServer(
            connectionString,
            sql => sql.EnableRetryOnFailure()));

        services.AddCorePersistence<OrderingDbContext>(typeof(OrderingRepository<>));
        services.AddScoped<IDbInitializerService, DbInitializerService>();

        return services;
    }
}
