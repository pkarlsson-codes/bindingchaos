namespace BindingChaos.CorePlatform.Contracts.Filters;

/// <summary>
/// Filter for querying societies.
/// </summary>
public record SocietiesQueryFilter
{
    /// <summary>
    /// Gets or sets a tag to filter societies by; only societies with this tag are returned.
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to filter to only societies that have geographic bounds.
    /// When null, no geographic filtering is applied.
    /// </summary>
    public bool? HasGeographicBounds { get; set; }
}
