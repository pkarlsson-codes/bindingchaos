namespace BindingChaos.CorePlatform.Contracts.Filters;

/// <summary>
/// Filter for querying ideas.
/// </summary>
public record IdeasQueryFilter
{
    /// <summary>
    /// Optional list of society IDs to scope the ideas to.
    /// </summary>
    public string[]? SocietyIds { get; set; }

    /// <summary>
    /// Search term to filter ideas by title or description.
    /// </summary>
    public string SearchTerm { get; set; } = string.Empty;

    /// <summary>
    /// Filter by the tags associated with the idea.
    /// </summary>
    public string[] Tags { get; set; } = [];
}
