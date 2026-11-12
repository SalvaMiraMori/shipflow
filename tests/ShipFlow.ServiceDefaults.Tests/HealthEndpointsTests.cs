using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace ShipFlow.ServiceDefaults.Tests;

public sealed class HealthEndpointsTests : IAsyncLifetime
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.AddServiceDefaults();

        _app = builder.Build();
        _app.MapDefaultEndpoints();
        await _app.StartAsync(TestContext.Current.CancellationToken);

        _client = _app.GetTestClient();
    }

    [Theory]
    [InlineData(Extensions.HealthEndpointPath)]
    [InlineData(Extensions.AlivenessEndpointPath)]
    public async Task HealthEndpoint_WhenServiceIsUp_ReturnsHealthy(string path)
    {
        var response = await _client.GetAsync(path, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldBe("Healthy");
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
    }
}
