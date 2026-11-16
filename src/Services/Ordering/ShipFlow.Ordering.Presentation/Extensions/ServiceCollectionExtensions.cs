using Microsoft.Extensions.DependencyInjection;

namespace ShipFlow.Ordering.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(AssemblyReference.Assembly);

        services.AddProblemDetails();

        return services;
    }
}
