using Core.Specification.Evaluators;
using Core.SpecificationEF.Evaluators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.SpecificationEF;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the default EF Core <see cref="ISpecificationEvaluator"/> (singleton, stateless).
    /// Call it before registering your own evaluator if you need a custom one; <c>TryAdd</c> keeps the first registration.
    /// </summary>
    public static IServiceCollection AddCoreSpecification(this IServiceCollection services, bool cacheEnabled = true)
    {
        services.TryAddSingleton<ISpecificationEvaluator>(cacheEnabled ? SpecificationEvaluator.Cached : SpecificationEvaluator.Default);
        return services;
    }
}
