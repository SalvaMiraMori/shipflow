using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using ShipFlow.Ordering.Persistence.Data;
using Testcontainers.MsSql;

namespace ShipFlow.Ordering.Api.IntegrationTests;

/// <summary>
/// Runs the real API against SQL Server in Docker (Testcontainers), as CLAUDE.md requires: no in-memory provider.
/// The schema is created on start-up by DbInitializerService (Development environment).
/// </summary>
public sealed class OrderingApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public async ValueTask InitializeAsync() => await _sqlServer.StartAsync(TestContext.Current.CancellationToken);

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await _sqlServer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting($"ConnectionStrings:{OrderingDbContext.ConnectionStringName}", _sqlServer.GetConnectionString());
    }
}
