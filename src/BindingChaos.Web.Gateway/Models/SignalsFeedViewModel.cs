using BindingChaos.Infrastructure.API;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for the signals feed page, including signals and metadata for filtering.
/// </summary>
internal sealed class SignalsFeedViewModel
{
    /// <summary>
    /// The paginated signals data.
    /// </summary>
    public PaginatedResponse<SignalViewModel> Signals { get; set; } = new();

    /// <summary>
    /// All available tags across all signals (for filtering).
    /// </summary>
    public string[] AvailableTags { get; set; } = [];

    /// <summary>
    /// The current filter state applied to this response.
    /// </summary>
    public FilterStateViewModel AppliedFilters { get; set; } = new();
}

/// <summary>
/// Represents the filter state that was applied to generate this response.
/// </summary>
internal sealed class FilterStateViewModel
{
    /// <summary>
    /// The time range filter that was applied.
    /// </summary>
    public string TimeRange { get; set; } = "all";

    /// <summary>
    /// The amplification level filter that was applied.
    /// </summary>
    public string AmplificationLevel { get; set; } = "all";

    /// <summary>
    /// The search term that was applied.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// The tags that were filtered by.
    /// </summary>
    public string[] Tags { get; set; } = [];

    /// <summary>
    /// The sort option that was applied.
    /// </summary>
    public string SortBy { get; set; } = "recent";
}
