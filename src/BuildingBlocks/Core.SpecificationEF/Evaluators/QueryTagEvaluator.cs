using Core.Specification;
using Core.Specification.Builder;
using Core.Specification.Evaluators;
using Microsoft.EntityFrameworkCore;

namespace Core.SpecificationEF.Evaluators;

/// <summary>
/// Applies the tag set with <see cref="QueryTagBuilderExtensions.TagWith{T}(ISpecificationBuilder{T}, string)"/>
/// as an EF Core <c>TagWith</c> SQL comment. Part of the default pipeline.
/// </summary>
public sealed class QueryTagEvaluator : IEvaluator
{
    private QueryTagEvaluator() { }

    public static QueryTagEvaluator Instance { get; } = new();

    public bool IsCriteriaEvaluator => true;

    public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
    {
        if (specification.Items.TryGetValue(QueryTagBuilderExtensions.TagKey, out var value) && value is string tag)
        {
            query = query.TagWith(tag);
        }

        return query;
    }
}
