using Core.Application.Repositories;
using Core.SpecificationEF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Persistence;

public static class DependencyInjection
{
    /// <summary>
    /// Registers <see cref="IUnitOfWork"/> for <typeparamref name="TContext"/>, the default specification evaluator and
    /// the service's open generic repository (e.g. <c>typeof(OrderingRepository&lt;&gt;)</c>, a one-line subclass of
    /// <see cref="Repositories.EfRepository{T, TContext}"/>) as <see cref="IRepository{T}"/> and
    /// <see cref="IReadRepository{T}"/>. The DbContext itself is registered by the service.
    /// </summary>
    public static IServiceCollection AddCorePersistence<TContext>(this IServiceCollection services, Type openGenericRepository)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(openGenericRepository);
        if (!openGenericRepository.IsGenericTypeDefinition || openGenericRepository.GetGenericArguments().Length != 1)
        {
            throw new ArgumentException(
                $"{openGenericRepository.Name} must be an open generic type with one type parameter, e.g. typeof(MyRepository<>).",
                nameof(openGenericRepository));
        }

        services.AddCoreSpecification();
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IUnitOfWork, EfUnitOfWork<TContext>>();

        services.TryAddScoped(typeof(IRepository<>), openGenericRepository);
        services.TryAddScoped(typeof(IReadRepository<>), openGenericRepository);

        return services;
    }
}
