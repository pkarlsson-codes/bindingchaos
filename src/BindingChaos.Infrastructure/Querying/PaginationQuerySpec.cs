using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BindingChaos.Infrastructure.Querying;

/// <summary>
/// Query specification for HTTP querystring binding with offset pagination, sorting, and typed filters.
/// </summary>
/// <typeparam name="TFilter">The type that represents custom filters for the query (e.g., a record or DTO).</typeparam>
public sealed class PaginationQuerySpec<TFilter>
    where TFilter : class, new()
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
    /// Typed filter payload bound from the filter.* prefix (e.g., filter.status).
    /// </summary>
    public TFilter Filter { get; init; } = new();

    /// <summary>
    /// Parsed sort descriptors bound from the querystring parameter named 'sort'.
    /// </summary>
    [FromQuery(Name = "sort")]
    [ModelBinder(BinderType = typeof(SortDescriptorsModelBinder))]
    public IReadOnlyList<SortDescriptor> SortDescriptors { get; set; } = Array.Empty<SortDescriptor>();

    /// <summary>
    /// Returns a new instance with normalized pagination and sanitized sort string.
    /// </summary>
    /// <returns>A new normalized <see cref="PaginationQuerySpec{TFilter}"/> instance.</returns>
    public PaginationQuerySpec<TFilter> Normalize()
    {
        var normalizedPage = new PageSpec
        {
            Number = NormalizePageNumber(Page.Number),
            Size = NormalizePageSize(Page.Size),
        };

        return new PaginationQuerySpec<TFilter>
        {
            Page = normalizedPage,
            Filter = Filter,
            SortDescriptors = SanitizeSortDescriptors(SortDescriptors),
        };
    }

    /// <summary>
    /// Builds a querystring that reproduces this specification when bound by ASP.NET Core.
    /// </summary>
    /// <param name="includeQuestionMark">Whether to prefix with '?' (default: true).</param>
    /// <returns>A querystring like "?page.number=1&amp;page.size=20&amp;filter.status=active&amp;sort=createdAt,-amplifyCount".</returns>
    public string ToQueryString(bool includeQuestionMark = true)
    {
        var parts = new List<string>(capacity: 8);

        parts.Add($"page.number={Encode(Page.Number.ToString(System.Globalization.CultureInfo.InvariantCulture))}");
        parts.Add($"page.size={Encode(Page.Size.ToString(System.Globalization.CultureInfo.InvariantCulture))}");

        if (Filter is not null)
        {
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

    private static SortDescriptor[] SanitizeSortDescriptors(IReadOnlyList<SortDescriptor> input)
    {
        if (input == null || input.Count == 0)
        {
            return Array.Empty<SortDescriptor>();
        }

        return input
            .Where(s => s != null && !string.IsNullOrWhiteSpace(s.Field))
            .Select(s => new SortDescriptor(s.Field.Trim(), s.Direction))
            .ToArray();
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

    private static string? ConvertToString(object value)
    {
        switch (value)
        {
            case string s when !string.IsNullOrWhiteSpace(s):
                return s;
            case string:
                return null;
            case bool b:
                return b ? "true" : "false";
            case DateTime dt:
                return dt.ToString("O", System.Globalization.CultureInfo.InvariantCulture);
            case DateTimeOffset dto:
                return dto.ToString("O", System.Globalization.CultureInfo.InvariantCulture);
            case Enum e:
                return e.ToString();
            case IFormattable fmt:
                return fmt.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            case IEnumerable<string> strings:
                return string.Join(',', strings);
            default:
                return value.ToString();
        }
    }

    private static string ToCamelCase(string name)
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

    private static string Encode(string value)
    {
        return Uri.EscapeDataString(value);
    }
}
