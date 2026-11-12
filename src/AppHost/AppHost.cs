var builder = DistributedApplication.CreateBuilder(args);

// Phase 1: Ordering API + Azure SQL
// var sql = builder.AddSqlServer("sql").AddDatabase("ordering-db");
// builder.AddProject<Projects.Ordering_Api>("ordering-api")
//        .WithReference(sql).WaitFor(sql);

// Phase 2: Inventory + Shipping (PostgreSQL), YARP gateway
// Phase 3: RabbitMQ (local) / Azure Service Bus (cloud)

builder.Build().Run();
