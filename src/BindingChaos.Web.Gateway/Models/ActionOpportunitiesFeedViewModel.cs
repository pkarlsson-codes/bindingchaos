using BindingChaos.Infrastructure.API;

namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// View model for the action opportunities feed page, including action opportunities and metadata for filtering.
/// </summary>
internal sealed class ActionOpportunitiesFeedViewModel
{
    /// <summary>
    /// The paginated action opportunities data.
    /// </summary>
    public PaginatedResponse<ActionOpportunityListResponse> ActionOpportunities { get; set; } = new();

    /// <summary>
    /// All available statuses across all action opportunities (for filtering).
    /// </summary>
    public string[] AvailableStatuses { get; set; } = [];

    /// <summary>
    /// The current filter state applied to this response.
    /// </summary>
    public ActionOpportunitiesFilterStateViewModel AppliedFilters { get; set; } = new();
}

/// <summary>
/// Represents the filter state that was applied to generate this response.
/// </summary>
internal sealed class ActionOpportunitiesFilterStateViewModel
{
    /// <summary>
    /// The search term that was applied.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// The status filter that was applied.
    /// </summary>
    public string Status { get; set; } = "all";

    /// <summary>
    /// The parent idea ID filter that was applied.
    /// </summary>
    public string? ParentIdeaId { get; set; }

    /// <summary>
    /// The sort option that was applied.
    /// </summary>
    public string SortBy { get; set; } = "recent";
}
