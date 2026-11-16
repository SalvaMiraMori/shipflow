using System.Collections.Concurrent;
using Core.Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Events;

/// <summary>
/// Resolves every <see cref="IDomainEventHandler{TDomainEvent}"/> for each event from the current scope.
/// </summary>
public sealed class DomainEventDispatcher(IServiceProvider serviceProvider) : IDomainEventDispatcher
{
    private static readonly ConcurrentDictionary<Type, Func<object, IDomainEvent, CancellationToken, Task>> _invokers = new();

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
            var invoke = _invokers.GetOrAdd(eventType, CreateInvoker);

            foreach (var handler in serviceProvider.GetServices(handlerType))
            {
                if (handler is not null)
                {
                    await invoke(handler, domainEvent, cancellationToken);
                }
            }
        }
    }

    private static Func<object, IDomainEvent, CancellationToken, Task> CreateInvoker(Type eventType)
    {
        var method = typeof(DomainEventDispatcher)
            .GetMethod(nameof(InvokeHandler), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .MakeGenericMethod(eventType);

        return (Func<object, IDomainEvent, CancellationToken, Task>)Delegate.CreateDelegate(
            typeof(Func<object, IDomainEvent, CancellationToken, Task>), method);
    }

    private static Task InvokeHandler<TDomainEvent>(object handler, IDomainEvent domainEvent, CancellationToken cancellationToken)
        where TDomainEvent : IDomainEvent =>
        ((IDomainEventHandler<TDomainEvent>)handler).HandleAsync((TDomainEvent)domainEvent, cancellationToken);
}
