using System.Reflection;
using Core.Application.Behaviors;
using Core.Application.Events;
using Core.Application.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers every command handler, query handler, domain event handler and FluentValidation validator found in
    /// <paramref name="assemblies"/>, and wraps command handlers with the standard pipeline:
    /// Logging → Validation → UnitOfWork → handler.
    /// </summary>
    public static IServiceCollection AddCoreApplication(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentOutOfRangeException.ThrowIfZero(assemblies.Length);

        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)).Where(IsNotCoreDecorator), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Scoped, includeInternalTypes: true);

        // Decorate registers outermost last: the call order below yields Logging(Validation(UnitOfWork(handler))).
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(UnitOfWorkCommandHandlerDecorator<,>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationCommandHandlerDecorator<,>));
        services.TryDecorate(typeof(ICommandHandler<,>), typeof(LoggingCommandHandlerDecorator<,>));

        return services;
    }

    private static bool IsNotCoreDecorator(Type type) => type.Assembly != typeof(DependencyInjection).Assembly;
}
