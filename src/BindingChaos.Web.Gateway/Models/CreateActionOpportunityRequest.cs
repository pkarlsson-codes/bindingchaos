namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Request model for creating a new action opportunity.
/// </summary>
public sealed class CreateActionOpportunityRequest
{
    /// <summary>
    /// The title of the action opportunity.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The description of the action opportunity.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the parent idea that spawned this action opportunity.
    /// </summary>
    public string ParentIdeaId { get; set; } = string.Empty;
}