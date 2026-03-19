using BindingChaos.Infrastructure.API;

namespace BindingChaos.CommunityDiscourse.Application.DTOs;

/// <summary>
/// Response model for root contributions with cursor-based pagination.
/// </summary>
public class RootContributionsView
{
    /// <summary>
    /// The type of entity this discourse is about.
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the entity this discourse is about.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// The cursor-paginated root contributions for this entity.
    /// </summary>
    public CursorPaginatedResponse<RootContribution> Contributions { get; set; } = new();

    /// <summary>
    /// Total number of all contributions for this entity (includes replies).
    /// </summary>
    public int TotalContributionCount { get; set; }

    /// <summary>
    /// Total number of root contributions for this entity.
    /// </summary>
    public int TotalRootContributionCount { get; set; }

    /// <summary>
    /// Total number of unique participants.
    /// </summary>
    public int TotalParticipants { get; set; }

    /// <summary>
    /// When the last contribution was posted.
    /// </summary>
    public DateTimeOffset LastActivityAt { get; set; }
}
