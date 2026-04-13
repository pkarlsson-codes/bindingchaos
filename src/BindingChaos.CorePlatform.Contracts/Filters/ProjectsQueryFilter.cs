namespace BindingChaos.CorePlatform.Contracts.Filters;

/// <summary>
/// Filter for querying projects.
/// </summary>
public record ProjectsQueryFilter
{
    /// <summary>
    /// Optional comma-separated list of statuses to filter by (e.g. <c>Active,Completed</c>).
    /// When omitted or empty, projects of all statuses are returned.
    /// </summary>
    public string? Statuses { get; set; }
}
