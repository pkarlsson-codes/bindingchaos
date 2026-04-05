namespace BindingChaos.CorePlatform.Contracts.Filters;

/// <summary>
/// Filter for querying ideas.
/// </summary>
public record IdeasQueryFilter
{
    /// <summary>
    /// Optional free-text search term to match against idea title or description.
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Optional status filter (e.g., "Draft", "Published").
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Optional participant ID to filter ideas authored by that participant.
    /// </summary>
    public string? AuthorId { get; set; }
}
