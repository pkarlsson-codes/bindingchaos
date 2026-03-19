using System.Linq.Expressions;
using BindingChaos.CommunityDiscourse.Application.ReadModels;
using BindingChaos.SharedKernel.Specifications;

namespace BindingChaos.CommunityDiscourse.Application.Specifications;

/// <summary>
/// Matches replies that belong to a specific parent contribution.
/// </summary>
internal sealed class RepliesForContributionSpecification(string contributionId) : Specification<ContributionView>
{
    private readonly string _contributionId = contributionId ?? throw new ArgumentNullException(nameof(contributionId));

    /// <inheritdoc/>
    public override Expression<Func<ContributionView, bool>> ToExpression()
    {
        return contribution => contribution.ParentContributionId == _contributionId;
    }
}

/// <summary>
/// Matches root contributions in a specific thread.
/// </summary>
internal sealed class RootContributionsInThreadSpecification(string threadId) : Specification<ContributionView>
{
    private readonly string _threadId = threadId ?? throw new ArgumentNullException(nameof(threadId));

    /// <inheritdoc/>
    public override Expression<Func<ContributionView, bool>> ToExpression()
    {
        return contribution => contribution.ThreadId == _threadId && contribution.IsRootContribution;
    }
}

/// <summary>
/// Matches contributions that come before a cursor in descending traversal order.
/// </summary>
internal sealed class ContributionBeforeCursorSpecification(DateTimeOffset cursorTimestamp, string cursorId) : Specification<ContributionView>
{
    private readonly DateTimeOffset _cursorTimestamp = cursorTimestamp;
    private readonly string _cursorId = cursorId ?? throw new ArgumentNullException(nameof(cursorId));

    /// <inheritdoc/>
    public override Expression<Func<ContributionView, bool>> ToExpression()
    {
        return contribution => contribution.CreatedAt < _cursorTimestamp ||
            (contribution.CreatedAt == _cursorTimestamp && string.Compare(contribution.Id, _cursorId, StringComparison.Ordinal) < 0);
    }
}

/// <summary>
/// Matches contributions that come after a cursor in ascending traversal order.
/// </summary>
internal sealed class ContributionAfterCursorSpecification(DateTimeOffset cursorTimestamp, string cursorId) : Specification<ContributionView>
{
    private readonly DateTimeOffset _cursorTimestamp = cursorTimestamp;
    private readonly string _cursorId = cursorId ?? throw new ArgumentNullException(nameof(cursorId));

    /// <inheritdoc/>
    public override Expression<Func<ContributionView, bool>> ToExpression()
    {
        return contribution => contribution.CreatedAt > _cursorTimestamp ||
            (contribution.CreatedAt == _cursorTimestamp && string.Compare(contribution.Id, _cursorId, StringComparison.Ordinal) > 0);
    }
}
