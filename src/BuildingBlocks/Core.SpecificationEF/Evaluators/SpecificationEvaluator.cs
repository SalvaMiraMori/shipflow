using Core.Specification;
using Core.Specification.Evaluators;
using Core.Specification.Exceptions;

namespace Core.SpecificationEF.Evaluators;

/// <inheritdoc/>
public class SpecificationEvaluator : ISpecificationEvaluator
{
    /// <summary>
    /// <see cref="SpecificationEvaluator" /> instance with default evaluators and without any additional features enabled.
    /// </summary>
    public static SpecificationEvaluator Default { get; } = new();

    /// <summary>
    /// <see cref="SpecificationEvaluator" /> instance with default evaluators and enabled caching.
    /// </summary>
    public static SpecificationEvaluator Cached { get; } = new(cacheEnabled: true);

    protected List<IEvaluator> Evaluators { get; } = [];

    public SpecificationEvaluator()
        : this(cacheEnabled: false)
    {
    }

    public SpecificationEvaluator(bool cacheEnabled)
    {
        Evaluators.AddRange(
        [
            WhereEvaluator.Instance,
            SearchEvaluator.Instance,
            cacheEnabled ? IncludeEvaluator.Cached : IncludeEvaluator.Default,
            OrderEvaluator.Instance,
            PaginationEvaluator.Instance,
            AsNoTrackingEvaluator.Instance,
            AsNoTrackingWithIdentityResolutionEvaluator.Instance,
            AsTrackingEvaluator.Instance,
            IgnoreQueryFiltersEvaluator.Instance,
            AsSplitQueryEvaluator.Instance,
            QueryTagEvaluator.Instance,
        ]);
    }

    /// <summary>
    /// Creates an evaluator with the default pipeline plus the given custom evaluators (appended at the end).
    /// Replaces the need to subclass the evaluator just to add one step (EcoGreen's <c>AppSpecificationEvaluator</c>).
    /// </summary>
    public SpecificationEvaluator(IEnumerable<IEvaluator> additionalEvaluators, bool cacheEnabled = false)
        : this(cacheEnabled)
    {
        Evaluators.AddRange(additionalEvaluators);
    }

    /// <inheritdoc/>
    public virtual IQueryable<TResult> GetQuery<T, TResult>(IQueryable<T> query, ISpecification<T, TResult> specification) where T : class
    {
        ArgumentNullException.ThrowIfNull(specification);
        if (specification.Selector is null && specification.SelectorMany is null)
        {
            throw new SelectorNotFoundException();
        }

        if (specification.Selector is not null && specification.SelectorMany is not null)
        {
            throw new ConcurrentSelectorsException();
        }

        query = GetQuery(query, (ISpecification<T>)specification);

        return specification.Selector is not null
          ? query.Select(specification.Selector)
          : query.SelectMany(specification.SelectorMany!);
    }

    /// <inheritdoc/>
    public virtual IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification, bool evaluateCriteriaOnly = false) where T : class
    {
        ArgumentNullException.ThrowIfNull(specification);

        var evaluators = evaluateCriteriaOnly ? Evaluators.Where(x => x.IsCriteriaEvaluator) : Evaluators;

        foreach (var evaluator in evaluators)
        {
            query = evaluator.GetQuery(query, specification);
        }

        return query;
    }
}
