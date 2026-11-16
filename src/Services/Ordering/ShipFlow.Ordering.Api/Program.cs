using ShipFlow.Ordering.Application.Extensions;
using ShipFlow.Ordering.Domain.Extensions;
using ShipFlow.Ordering.Infrastructure.Extensions;
using ShipFlow.Ordering.Persistence.Extensions;
using ShipFlow.Ordering.Persistence.Services.DbInitializer;
using ShipFlow.Ordering.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry, health checks, service discovery, resilience (replaces EcoGreen's Serilog + App Insights sink).
builder.AddServiceDefaults();

builder.Services
    .AddPresentation()
    .AddDomain()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<IDbInitializerService>().InitializeAsync();
}

// Unhandled exceptions → 500 Problem Details (no stack traces to clients); validation/domain errors are mapped by ApiController.
app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseCors(ShipFlow.Ordering.Infrastructure.Extensions.ServiceCollectionExtensions.SpaCorsPolicy);

app.MapControllers();
app.MapDefaultEndpoints();

await app.RunAsync();

/// <summary>Entry point, exposed for <c>WebApplicationFactory</c> integration tests.</summary>
public partial class Program;
