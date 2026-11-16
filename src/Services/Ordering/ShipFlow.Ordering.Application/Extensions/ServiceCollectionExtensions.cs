using Core.Application;
using Microsoft.Extensions.DependencyInjection;

namespace ShipFlow.Ordering.Application.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers handlers, validators and domain event handlers of this assembly with the Core pipeline
    /// (Logging → Validation → UnitOfWork). Replaces EcoGreen's hand-written Scrutor scan.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services.AddCoreApplication(AssemblyReference.Assembly);
}
