using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Query specification for HTTP querystring binding with offset pagination and sorting.
/// </summary>
public class PaginationQuerySpec
{
    /// <summary>
    /// Default page size when none is specified.
    /// </summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    /// Maximum allowed page size. Consumers may clamp values above this limit.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// Page parameters bound from querystring using the page.* prefix (e.g., page.number, page.size).
    /// </summary>
    public PageSpec Page { get; init; } = new();

    /// <summary>
    /// Parsed sort descriptors bound from the querystring parameter named 'sort'.
    /// </summary>
    [FromQuery(Name = "sort")]
    [ModelBinder(BinderType = typeof(SortDescriptorsModelBinder))]
    public IReadOnlyList<SortDescriptor> SortDescriptors { get; set; } = Array.Empty<SortDescriptor>();

    /// <summary>
    /// Returns a new instance with normalized pagination and sanitized sort descriptors.
    /// </summary>
    /// <returns>A new normalized <see cref="PaginationQuerySpec"/> instance.</returns>
    public virtual PaginationQuerySpec Normalize()
    {
        return new PaginationQuerySpec
        {
            Page = NormalizedPage(),
            SortDescriptors = SanitizeSortDescriptors(SortDescriptors),
        };
    }

    /// <summary>
    /// Builds a querystring that reproduces this specification when bound by ASP.NET Core.
    /// </summary>
    /// <param name="includeQuestionMark">Whether to prefix with '?' (default: true).</param>
    /// <returns>A querystring like "?page.number=1&amp;page.size=20&amp;sort=createdAt,-amplifyCount".</returns>
    public string ToQueryString(bool includeQuestionMark = true)
    {
        var parts = new List<string>(capacity: 8)
        {
            $"page.number={Encode(Page.Number.ToString(System.Globalization.CultureInfo.InvariantCulture))}",
            $"page.size={Encode(Page.Size.ToString(System.Globalization.CultureInfo.InvariantCulture))}",
        };

        AppendFilterParts(parts);

        if (SortDescriptors is { Count: > 0 })
        {
            var sort = BuildSortString(SortDescriptors);
            if (!string.IsNullOrWhiteSpace(sort))
            {
                parts.Add($"sort={Encode(sort)}");
            }
        }

        var builder = new StringBuilder(parts.Count * 16);
        if (includeQuestionMark)
        {
            builder.Append('?');
        }

        for (int i = 0; i < parts.Count; i++)
        {
            if (i > 0)
            {
                builder.Append('&');
            }

            builder.Append(parts[i]);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Builds a <see cref="QueryString"/> instance representing this specification.
    /// </summary>
    /// <returns>A <see cref="QueryString"/> beginning with a leading '?'.</returns>
    public QueryString ToQueryStringValue()
    {
        var text = ToQueryString(includeQuestionMark: true);
        return new QueryString(text);
    }

    /// <summary>
    /// Sanitizes a list of sort descriptors by removing nulls and trimming field names.
    /// </summary>
    /// <param name="input">The raw sort descriptors to sanitize.</param>
    /// <returns>A sanitized array of <see cref="SortDescriptor"/> instances.</returns>
    protected static SortDescriptor[] SanitizeSortDescriptors(IReadOnlyList<SortDescriptor> input)
    {
        if (input == null || input.Count == 0)
        {
            return [];
        }

        return [.. input
            .Where(s => s != null && !string.IsNullOrWhiteSpace(s.Field))
            .Select(s => new SortDescriptor(s.Field.Trim(), s.Direction))];
    }

    /// <summary>
    /// Converts a filter property value to its querystring string representation.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The string representation, or <c>null</c> if the value should be omitted.</returns>
    protected static string? ConvertToString(object value)
    {
        return value switch
        {
            string s when !string.IsNullOrWhiteSpace(s) => s,
            string => null,
            bool b => b ? "true" : "false",
            DateTime dt => dt.ToString("O", System.Globalization.CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("O", System.Globalization.CultureInfo.InvariantCulture),
            Enum e => e.ToString(),
            IFormattable fmt => fmt.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
            IEnumerable<string> strings => string.Join(',', strings),
            _ => value.ToString(),
        };
    }

    /// <summary>
    /// Converts a PascalCase property name to camelCase for use as a querystring key.
    /// </summary>
    /// <param name="name">The PascalCase name to convert.</param>
    /// <returns>The camelCase equivalent.</returns>
    protected static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
        {
            return name;
        }

        if (name.Length == 1)
        {
            return name.ToLowerInvariant();
        }

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }

    /// <summary>
    /// URL-encodes a querystring value.
    /// </summary>
    /// <param name="value">The value to encode.</param>
    /// <returns>The percent-encoded string.</returns>
    protected static string Encode(string value)
    {
        return Uri.EscapeDataString(value);
    }

    /// <summary>
    /// Override to append filter-specific querystring parts between the page and sort segments.
    /// </summary>
    /// <param name="parts">The list of querystring key=value parts to append to.</param>
    protected virtual void AppendFilterParts(List<string> parts) { }

    /// <summary>
    /// Returns a normalized <see cref="PageSpec"/> derived from the current <see cref="Page"/>.
    /// </summary>
    /// <returns>A <see cref="PageSpec"/> with valid page number and clamped page size.</returns>
    protected PageSpec NormalizedPage() => new()
    {
        Number = NormalizePageNumber(Page.Number),
        Size = NormalizePageSize(Page.Size),
    };

    private static int NormalizePageNumber(int pageNumber)
    {
        return pageNumber < 1 ? 1 : pageNumber;
    }

    private static int NormalizePageSize(int pageSize)
    {
        if (pageSize < 1)
        {
            return DefaultPageSize;
        }

        return pageSize > MaxPageSize ? MaxPageSize : pageSize;
    }

    private static string BuildSortString(IReadOnlyList<SortDescriptor> descriptors)
    {
        if (descriptors == null || descriptors.Count == 0)
        {
            return string.Empty;
        }

        var tokens = descriptors
            .Where(d => d != null && !string.IsNullOrWhiteSpace(d.Field))
            .Select(d => d.Direction == SortDirection.Desc ? $"-{d.Field}" : d.Field)
            .ToArray();

        return string.Join(',', tokens);
    }
}
