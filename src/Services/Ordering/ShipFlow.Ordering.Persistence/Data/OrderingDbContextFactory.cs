using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShipFlow.Ordering.Persistence.Data;

/// <summary>
/// Used only by <c>dotnet ef</c> at design time. Reads <c>ConnectionStrings__ordering-db</c> or falls back to LocalDB.
/// </summary>
public sealed class OrderingDbContextFactory : IDesignTimeDbContextFactory<OrderingDbContext>
{
    private const string LocalDb = "Server=(localdb)\\MSSQLLocalDB;Database=shipflow-ordering;Trusted_Connection=True;TrustServerCertificate=True";

    public OrderingDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable($"ConnectionStrings__{OrderingDbContext.ConnectionStringName}") ?? LocalDb;

        var options = new DbContextOptionsBuilder<OrderingDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new OrderingDbContext(options);
    }
}
