namespace BindingChaos.CorePlatform.Contracts.Filters;

/// <summary>
/// Filter for querying amendments.
/// </summary>
public record AmendmentsQueryFilter
{
    /// <summary>
    /// Gets or sets the unique identifier of the idea associated with the amendments.
    /// </summary>
    public string IdeaId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status filter for amendments.
    /// </summary>
    public string? StatusFilter { get; set; }
}
