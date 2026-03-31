using System.Reflection;

namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Query specification for HTTP querystring binding with offset pagination, sorting, and typed filters.
/// </summary>
/// <typeparam name="TFilter">The type that represents custom filters for the query (e.g., a record or DTO).</typeparam>
public sealed class PaginationQuerySpec<TFilter> : PaginationQuerySpec
    where TFilter : class, new()
{
    /// <summary>
    /// Typed filter payload bound from the filter.* prefix (e.g., filter.status).
    /// </summary>
    public TFilter Filter { get; init; } = new();

    /// <summary>
    /// Returns a new instance with normalized pagination, sanitized sort descriptors, and preserved filter.
    /// </summary>
    /// <returns>A new normalized <see cref="PaginationQuerySpec{TFilter}"/> instance.</returns>
    public override PaginationQuerySpec<TFilter> Normalize()
    {
        return new PaginationQuerySpec<TFilter>
        {
            Page = NormalizedPage(),
            Filter = Filter,
            SortDescriptors = SanitizeSortDescriptors(SortDescriptors),
        };
    }

    /// <inheritdoc/>
    protected override void AppendFilterParts(List<string> parts)
    {
        if (Filter is null)
        {
            return;
        }

        foreach (var property in typeof(TFilter).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetMethod is null)
            {
                continue;
            }

            var value = property.GetValue(Filter);
            if (value is null)
            {
                continue;
            }

            var stringValue = ConvertToString(value);
            if (stringValue is null)
            {
                continue;
            }

            var key = $"filter.{ToCamelCase(property.Name)}";
            parts.Add($"{key}={Encode(stringValue)}");
        }
    }
}
