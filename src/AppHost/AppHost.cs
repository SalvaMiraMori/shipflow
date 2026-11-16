var builder = DistributedApplication.CreateBuilder(args);

// Phase 1: Ordering API + SQL Server (Azure SQL in the cloud).
// The resource name "ordering-db" becomes ConnectionStrings__ordering-db in the API.
var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var orderingDb = sql.AddDatabase("ordering-db");

builder.AddProject<Projects.ShipFlow_Ordering_Api>("ordering-api")
    .WithReference(orderingDb)
    .WaitFor(orderingDb);

// Phase 2: Inventory + Shipping (PostgreSQL), YARP gateway
// Phase 3: RabbitMQ (local) / Azure Service Bus (cloud)

builder.Build().Run();
