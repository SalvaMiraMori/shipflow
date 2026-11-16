using Core.Domain.Primitives;

namespace Core.Application.Repositories;

/// <summary>
/// Read + write repository, restricted to aggregate roots (DDD: one repository per aggregate).
/// Equivalent of EcoGreen's <c>IEcogreenBaseRepository&lt;T&gt;</c>, now generic for every service.
/// </summary>
public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
    where T : class, IAggregateRoot;
