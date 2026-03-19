using System.Linq.Expressions;

namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Extensions for applying sorting to queryables using a provided sort mapping.
/// </summary>
public static class QueryableSortExtensions
{
    /// <summary>
    /// Placeholder Sort extension. Applies sorting using the provided mapping.
    /// Currently a no-op; implementation to be added.
    /// </summary>
    /// <typeparam name="T">The read model type.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="descriptors">The sort descriptors.</param>
    /// <param name="mappings">Mapping of logical sort fields to key selectors.</param>
    /// <returns>The sorted query.</returns>
    public static IQueryable<T> Sort<T>(
        this IQueryable<T> source,
        IReadOnlyList<SortDescriptor>? descriptors,
        IReadOnlyDictionary<string, Expression<Func<T, object>>> mappings)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(mappings);

        if (descriptors == null || descriptors.Count == 0)
        {
            return source;
        }

        IOrderedQueryable<T>? ordered = null;

        foreach (var d in descriptors)
        {
            if (d == null || string.IsNullOrWhiteSpace(d.Field))
            {
                continue;
            }

            var key = d.Field.Trim();
            if (!mappings.TryGetValue(key, out var selector))
            {
                continue;
            }

            ordered = ordered == null
                ? (d.Direction == SortDirection.Desc
                    ? source.OrderByDescending(selector)
                    : source.OrderBy(selector))
                : (d.Direction == SortDirection.Desc
                    ? ordered.ThenByDescending(selector)
                    : ordered.ThenBy(selector));
        }

        return ordered != null ? ordered : source;
    }
}
