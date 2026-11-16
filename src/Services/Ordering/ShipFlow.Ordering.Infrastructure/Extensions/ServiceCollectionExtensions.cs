using Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ShipFlow.Ordering.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public const string SpaCorsPolicy = "spa";
    private const string AllowedOriginsKey = "Cors:AllowedOrigins";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoreInfrastructure();

        // Angular SPA (Phase 5). Origins come from configuration, e.g. Cors__AllowedOrigins__0=https://localhost:4200.
        var allowedOrigins = configuration.GetSection(AllowedOriginsKey).Get<string[]>() ?? [];
        services.AddCors(options => options.AddPolicy(SpaCorsPolicy, policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

        return services;
    }
}
