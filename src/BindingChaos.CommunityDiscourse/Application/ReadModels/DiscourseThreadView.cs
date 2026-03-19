namespace BindingChaos.CommunityDiscourse.Application.ReadModels;

/// <summary>
/// Thread metadata document for querying thread-level information.
/// </summary>
public class DiscourseThreadView
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the entity this thread is for.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique entity identifier this thread is for.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the total number of contributions made.
    /// </summary>
    public int TotalContributions { get; set; }

    /// <summary>
    /// Gets or sets the total contributions from all root-level entities.
    /// </summary>
    public int TotalRootContributions { get; set; }

    /// <summary>
    /// Gets or sets the total number of participants.
    /// </summary>
    public int TotalParticipants { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the most recent activity.
    /// </summary>
    public DateTimeOffset LastActivityAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of participant identifiers.
    /// </summary>
    public List<string> ParticipantIds { get; set; } = [];
}
