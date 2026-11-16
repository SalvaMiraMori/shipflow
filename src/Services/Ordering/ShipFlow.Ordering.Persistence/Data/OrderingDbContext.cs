using Core.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using ShipFlow.Ordering.Domain.Application.Orders;
using ShipFlow.Ordering.Persistence.Constants;

namespace ShipFlow.Ordering.Persistence.Data;

public sealed class OrderingDbContext(DbContextOptions<OrderingDbContext> options) : DbContext(options)
{
    public const string ConnectionStringName = "ordering-db";

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaNames.Ordering);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
        modelBuilder.ApplySoftDeleteQueryFilter();
    }
}
