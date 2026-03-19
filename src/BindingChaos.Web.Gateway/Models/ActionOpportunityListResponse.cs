namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Response model for action opportunity list items.
/// </summary>
internal sealed class ActionOpportunityListResponse
{
    /// <summary>
    /// The unique identifier of the action opportunity.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The title of the action opportunity.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A brief description of the action opportunity.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The status of the action opportunity.
    /// </summary>
    public string Status { get; set; } = "emerging";

    /// <summary>
    /// The ID of the parent idea that spawned this action opportunity.
    /// </summary>
    public string ParentIdeaId { get; set; } = string.Empty;

    /// <summary>
    /// The title of the parent idea.
    /// </summary>
    public string ParentIdeaTitle { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the creator.
    /// </summary>
    public string CreatorId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the creator.
    /// </summary>
    public string CreatorName { get; set; } = string.Empty;

    /// <summary>
    /// The creation timestamp in ISO 8601 format.
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// The last update timestamp in ISO 8601 format.
    /// </summary>
    public string UpdatedAt { get; set; } = string.Empty;

    /// <summary>
    /// The number of participants committed to this action opportunity.
    /// </summary>
    public int ParticipantCount { get; set; }

    /// <summary>
    /// The current progress percentage (0-100).
    /// </summary>
    public int ProgressPercentage { get; set; }

    /// <summary>
    /// Whether the current user has committed to this action opportunity.
    /// </summary>
    public bool IsCommitted { get; set; }

    /// <summary>
    /// The current user's commitment type if committed.
    /// </summary>
    public string? UserCommitmentType { get; set; }
}