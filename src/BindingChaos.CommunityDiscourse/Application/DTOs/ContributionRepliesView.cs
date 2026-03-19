using BindingChaos.Infrastructure.API;

namespace BindingChaos.CommunityDiscourse.Application.DTOs;

/// <summary>
/// Response model for contribution replies (maintains API compatibility).
/// </summary>
public class ContributionRepliesView
{
    /// <summary>
    /// The ID of the parent contribution these replies belong to.
    /// </summary>
    public string ContributionId { get; set; } = string.Empty;

    /// <summary>
    /// The cursor-paginated replies to this contribution.
    /// </summary>
    public CursorPaginatedResponse<ContributionReply> Replies { get; set; } = new();
}
