namespace BindingChaos.Ideation.Application.ReadModels;

/// <summary>
/// Read model entity for ideas optimized for querying.
/// </summary>
public class IdeaView
{
    /// <summary>
    /// The unique identifier for the idea.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The creator of this idea.
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// The current status of this idea.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// The society context (governance jurisdiction) for this idea.
    /// </summary>
    public string SocietyContext { get; set; } = string.Empty;

    /// <summary>
    /// When this idea was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The tags associated with this idea.
    /// </summary>
    public List<string> Tags { get; internal set; } = [];

    /// <summary>
    /// The signal references associated with this idea.
    /// </summary>
    public List<string> SignalReferenceIds { get; internal set; } = [];

    /// <summary>
    /// The parent idea ID if this idea was forked from another idea.
    /// </summary>
    public string? ParentIdeaId { get; set; }

    /// <summary>
    /// The current version title of this idea.
    /// </summary>
    public string CurrentTitle { get; set; } = string.Empty;

    /// <summary>
    /// The current version body of this idea.
    /// </summary>
    public string CurrentBody { get; set; } = string.Empty;

    /// <summary>
    /// The current version number of this idea.
    /// </summary>
    public int CurrentVersionNumber { get; set; }

    /// <summary>
    /// The version number for optimistic concurrency control.
    /// </summary>
    public long Version { get; set; }

    /// <summary>
    /// When this idea was last updated.
    /// </summary>
    public DateTimeOffset LastUpdatedAt { get; set; }
}
