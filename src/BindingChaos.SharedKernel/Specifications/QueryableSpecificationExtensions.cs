namespace BindingChaos.SharedKernel.Specifications;

/// <summary>
/// Extension methods for applying specifications to queryable sources.
/// </summary>
public static class QueryableSpecificationExtensions
{
    /// <summary>
    /// Filters a queryable source to elements that satisfy the provided specification.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="query">The queryable source.</param>
    /// <param name="specification">The specification to apply.</param>
    /// <returns>A filtered queryable source.</returns>
    public static IQueryable<T> Matching<T>(this IQueryable<T> query, Specification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(specification);

        return query.Where(specification.ToExpression());
    }
}
