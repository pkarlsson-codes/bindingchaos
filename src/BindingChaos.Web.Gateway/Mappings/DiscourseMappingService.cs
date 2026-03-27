using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Infrastructure.API;
using BindingChaos.Web.Gateway.Models;

namespace BindingChaos.Web.Gateway.Mappings;

/// <summary>Pure structural mapping from discourse API responses to Gateway view models.</summary>
internal static class DiscourseMapper
{
    /// <summary>Maps a <see cref="GetContributionsResponse"/> to a <see cref="PostsViewModel"/>.</summary>
    /// <param name="result">The API response to map.</param>
    /// <returns>The mapped <see cref="PostsViewModel"/>.</returns>
    internal static PostsViewModel ToPostsViewModel(GetContributionsResponse result)
    {
        return new PostsViewModel
        {
            EntityType = result.EntityType,
            EntityId = result.EntityId,
            ThreadId = result.ThreadId,
            TotalPostCount = result.TotalContributionCount,
            TotalRootPostCount = result.TotalRootContributionCount,
            TotalParticipants = result.TotalParticipants,
            LastActivityAt = result.LastActivityAt,
            Posts = new CursorPaginatedResponse<PostViewModel>
            {
                Items = [.. result.Contributions.Items.Select(c => new PostViewModel
                {
                    Id = c.Id,
                    AuthorId = c.AuthorId,
                    AuthorPseudonym = c.AuthorPseudonym,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    ReplyCount = c.ReplyCount,
                    LastReplyAt = c.LastReplyAt,
                })],
                NextCursor = result.Contributions.NextCursor,
                PreviousCursor = result.Contributions.PreviousCursor,
                PageSize = result.Contributions.PageSize,
                RequestId = result.Contributions.RequestId,
                Timestamp = result.Contributions.Timestamp,
            },
        };
    }

    /// <summary>Maps a <see cref="GetContributionRepliesResponse"/> to a <see cref="PostRepliesViewModel"/>.</summary>
    /// <param name="result">The API response to map.</param>
    /// <returns>The mapped <see cref="PostRepliesViewModel"/>.</returns>
    internal static PostRepliesViewModel ToPostRepliesViewModel(GetContributionRepliesResponse result)
    {
        return new PostRepliesViewModel
        {
            PostId = result.ContributionId,
            Replies = new CursorPaginatedResponse<ReplyViewModel>
            {
                Items = result.Replies.Items.Select(r => new ReplyViewModel
                {
                    Id = r.Id,
                    AuthorId = r.AuthorId,
                    AuthorPseudonym = r.AuthorPseudonym,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    ParentPostId = r.ParentContributionId,
                    ReplyCount = r.ReplyCount,
                    HasMoreReplies = r.HasMoreReplies,
                }).ToArray(),
                NextCursor = result.Replies.NextCursor,
                PreviousCursor = result.Replies.PreviousCursor,
                PageSize = result.Replies.PageSize,
                RequestId = result.Replies.RequestId,
                Timestamp = result.Replies.Timestamp,
            },
        };
    }
}
