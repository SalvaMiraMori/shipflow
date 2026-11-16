using Microsoft.Extensions.DependencyInjection;

namespace ShipFlow.Ordering.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Domain services registration point (same entry point per layer as EcoGreen). Empty for now: the Order aggregate
    /// needs no injected services.
    /// </summary>
    public static IServiceCollection AddDomain(this IServiceCollection services) => services;
}
