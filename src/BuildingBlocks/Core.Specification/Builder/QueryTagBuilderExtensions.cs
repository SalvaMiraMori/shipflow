namespace Core.Specification.Builder;

/// <summary>
/// Generic query-tag support. In EcoGreen every aggregate had to write its own <c>TagWith</c> extension and the
/// persistence layer had to register a custom evaluator; here it works for any specification out of the box.
/// </summary>
public static class QueryTagBuilderExtensions
{
    /// <summary>The key used in <see cref="ISpecification{T}.Items"/> to store the query tag.</summary>
    public const string TagKey = "TagWith";

    /// <summary>
    /// Adds a tag to the generated SQL (<c>-- tag</c> comment), useful to find the query in logs and traces.
    /// </summary>
    public static ISpecificationBuilder<T> TagWith<T>(this ISpecificationBuilder<T> builder, string tag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);
        builder.Specification.Items[TagKey] = tag;
        return builder;
    }

    /// <summary>
    /// Adds a tag to the generated SQL, keeping the ordered builder so <c>ThenBy</c> can still be chained.
    /// </summary>
    public static IOrderedSpecificationBuilder<T> TagWith<T>(this IOrderedSpecificationBuilder<T> builder, string tag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag);
        builder.Specification.Items[TagKey] = tag;
        return builder;
    }
}
