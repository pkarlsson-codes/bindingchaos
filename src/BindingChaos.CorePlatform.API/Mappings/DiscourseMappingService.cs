using BindingChaos.CommunityDiscourse.Application.DTOs;
using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;

namespace BindingChaos.CorePlatform.API.Mappings;

/// <summary>Pure structural mapping from discourse read models to API response contracts.</summary>
internal static class DiscourseMapper
{
    /// <summary>Maps a <see cref="RootContributionsView"/> to a <see cref="GetContributionsResponse"/>.</summary>
    /// <param name="result">The read model to map.</param>
    /// <param name="requestId">The current request trace identifier.</param>
    /// <param name="threadId">Optional thread ID to include in the response.</param>
    /// <returns>The mapped <see cref="GetContributionsResponse"/>.</returns>
    internal static GetContributionsResponse ToGetContributionsResponse(
        RootContributionsView result,
        string requestId,
        string? threadId = null)
    {
        return new GetContributionsResponse
        {
            EntityType = result.EntityType,
            EntityId = result.EntityId,
            ThreadId = threadId ?? string.Empty,
            TotalContributionCount = result.TotalContributionCount,
            TotalRootContributionCount = result.TotalRootContributionCount,
            TotalParticipants = result.TotalParticipants,
            LastActivityAt = result.LastActivityAt,
            Contributions = new CursorPaginatedResponse<ContributionResponse>
            {
                Items = [.. result.Contributions.Items.Select(c => new ContributionResponse
                {
                    Id = c.Id,
                    AuthorId = c.AuthorId,
                    AuthorPseudonym = c.AuthorPseudonym,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ReplyCount = c.ReplyCount,
                    LastReplyAt = c.LastReplyAt,
                    HasReplies = c.HasReplies,
                })],
                NextCursor = result.Contributions.NextCursor,
                PreviousCursor = result.Contributions.PreviousCursor,
                PageSize = result.Contributions.PageSize,
                RequestId = requestId,
                Timestamp = DateTimeOffset.UtcNow,
            },
        };
    }

    /// <summary>Maps a <see cref="ContributionRepliesView"/> to a <see cref="GetContributionRepliesResponse"/>.</summary>
    /// <param name="result">The read model to map.</param>
    /// <param name="requestId">The current request trace identifier.</param>
    /// <returns>The mapped <see cref="GetContributionRepliesResponse"/>.</returns>
    internal static GetContributionRepliesResponse ToGetContributionRepliesResponse(
        ContributionRepliesView result,
        string requestId)
    {
        return new GetContributionRepliesResponse
        {
            ContributionId = result.ContributionId,
            Replies = new CursorPaginatedResponse<ContributionReplyResponse>
            {
                Items = [.. result.Replies.Items.Select(r => new ContributionReplyResponse
                {
                    Id = r.Id,
                    AuthorId = r.AuthorId,
                    AuthorPseudonym = r.AuthorPseudonym,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    ParentContributionId = r.ParentContributionId,
                    ReplyCount = r.ReplyCount,
                    HasMoreReplies = r.HasMoreReplies,
                })],
                NextCursor = result.Replies.NextCursor,
                PreviousCursor = result.Replies.PreviousCursor,
                PageSize = result.Replies.PageSize,
                RequestId = requestId,
                Timestamp = DateTimeOffset.UtcNow,
            },
        };
    }
}
