using BindingChaos.CommunityDiscourse.Application.DTOs;
using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.CommunityDiscourse.Application.Specifications;
using BindingChaos.Infrastructure.API;
using BindingChaos.Infrastructure.Querying;
using BindingChaos.SharedKernel.Specifications;
using Marten;

namespace BindingChaos.CommunityDiscourse.Application.Queries;

/// <summary>
/// Query to get root contributions for a specific thread.
/// </summary>
public sealed record GetRootContributions
{
    /// <summary>
    /// Gets the ID of the thread.
    /// </summary>
    public string ThreadId { get; }

    /// <summary>
    /// Gets the cursor for pagination.
    /// </summary>
    public string? Cursor { get; }

    /// <summary>
    /// Gets the maximum number of contributions to return.
    /// </summary>
    public int Limit { get; }

    /// <summary>
    /// Gets the direction of pagination.
    /// </summary>
    public CursorDirection Direction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetRootContributions"/> class.
    /// </summary>
    /// <param name="threadId">The ID of the thread.</param>
    /// <param name="cursor">The cursor for pagination.</param>
    /// <param name="limit">The maximum number of contributions to return.</param>
    /// <param name="direction">The direction of pagination.</param>
    public GetRootContributions(
        string threadId,
        string? cursor,
        int limit,
        CursorDirection direction)
    {
        ThreadId = threadId ?? throw new ArgumentNullException(nameof(threadId));
        Cursor = cursor;
        Limit = limit;
        Direction = direction;
    }
}

/// <summary>
/// Query handler for getting root contributions by thread ID.
/// </summary>
public static class GetRootContributionsHandler
{
    /// <summary>
    /// Retrieves a paginated list of root contributions for the specified thread.
    /// </summary>
    /// <param name="query">The query containing the thread ID, cursor, limit, and direction.</param>
    /// <param name="querySession">The read-only query session for accessing the read model.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="RootContributionsView"/> if the thread exists; otherwise, <see langword="null"/>.</returns>
    public static async Task<RootContributionsView?> Handle(
        GetRootContributions query,
        IQuerySession querySession,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var threadView = await querySession.Query<DiscourseThreadView>()
            .Where(t => t.Id == query.ThreadId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (threadView == null)
        {
            return null;
        }

        var specification = ContributionCursorHelpers.BuildContributionSpecification(
            new RootContributionsInThreadSpecification(query.ThreadId), query.Cursor, query.Direction);

        var baseQuery = querySession.Query<ContributionView>().Matching(specification);
        var orderedQuery = ContributionCursorHelpers.ApplyDirectionalOrdering(baseQuery, query.Direction);
        var (contributionDocuments, hasMore) = await ContributionCursorHelpers.ExecuteCursorPageAsync(
            orderedQuery, query.Limit, query.Direction, cancellationToken).ConfigureAwait(false);

        var contributions = contributionDocuments.Select(doc => new RootContribution
        {
            Id = doc.Id,
            AuthorId = doc.AuthorId,
            Content = doc.Content,
            CreatedAt = doc.CreatedAt,
            ReplyCount = doc.ReplyCount,
            LastReplyAt = doc.LastReplyAt,
        }).ToArray();

        var nextCursor = hasMore && query.Direction == CursorDirection.Forward
            ? CursorHelper.CreateNextCursor(contributions, c => c.CreatedAt, c => c.Id)
            : null;

        var previousCursor = hasMore && query.Direction == CursorDirection.Backward
            ? CursorHelper.CreatePreviousCursor(contributions, c => c.CreatedAt, c => c.Id)
            : null;

        return new RootContributionsView
        {
            EntityType = threadView.EntityType,
            EntityId = threadView.EntityId,
            TotalContributionCount = threadView.TotalContributions,
            TotalRootContributionCount = threadView.TotalRootContributions,
            TotalParticipants = threadView.TotalParticipants,
            LastActivityAt = threadView.LastActivityAt,
            Contributions = new CursorPaginatedResponse<RootContribution>
            {
                Items = contributions,
                NextCursor = nextCursor,
                PreviousCursor = previousCursor,
                PageSize = query.Limit,
            },
        };
    }
}
