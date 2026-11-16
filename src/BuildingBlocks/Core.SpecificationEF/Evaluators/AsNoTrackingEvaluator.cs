using Core.Specification;
using Core.Specification.Evaluators;
using Microsoft.EntityFrameworkCore;

namespace Core.SpecificationEF.Evaluators;

public class AsNoTrackingEvaluator : IEvaluator
{
    public AsNoTrackingEvaluator() { }

    public static AsNoTrackingEvaluator Instance { get; } = new AsNoTrackingEvaluator();

    public bool IsCriteriaEvaluator { get; } = true;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}
