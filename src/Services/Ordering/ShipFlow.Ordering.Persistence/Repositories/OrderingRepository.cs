using Core.Domain.Primitives;
using Core.Persistence.Repositories;
using Core.Specification.Evaluators;
using ShipFlow.Ordering.Persistence.Data;

namespace ShipFlow.Ordering.Persistence.Repositories;

/// <summary>
/// The service's generic repository (EcoGreen's <c>EcogreenBaseRepository</c>): all behaviour comes from Core.
/// Add aggregate-specific repositories only for queries a specification cannot express.
/// </summary>
public class OrderingRepository<T>(OrderingDbContext dbContext, ISpecificationEvaluator evaluator)
    : EfRepository<T, OrderingDbContext>(dbContext, evaluator)
    where T : class, IAggregateRoot;
