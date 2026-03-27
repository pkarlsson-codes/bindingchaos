using BindingChaos.CommunityDiscourse.Domain.Contributions;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.CommunityDiscourse.Application.Commands;

/// <summary>
/// Represents a contribution posted by a participant, associated with a specific entity and optionally replying to
/// another contribution.
/// </summary>
public sealed record PostContribution
{
    /// <summary>
    /// Gets the ID of the thread to post to.
    /// </summary>
    public string ThreadId { get; }

    /// <summary>
    /// Gets the ID of the participant posting the contribution.
    /// </summary>
    public ParticipantId AuthorId { get; }

    /// <summary>
    /// Gets the content of the contribution.
    /// </summary>
    public string Content { get; }

    /// <summary>
    /// Gets the ID of the parent contribution, if this is a reply to another contribution.
    /// </summary>
    public ContributionId? ParentContributionId { get; }

    /// <summary>
    /// Initializes a new instance of the PostContributionToThread class.
    /// </summary>
    /// <param name="threadId">The ID of the thread to post to.</param>
    /// <param name="authorId">The ID of the participant posting the contribution.</param>
    /// <param name="content">The content of the contribution.</param>
    /// <param name="parentContributionId">The ID of the parent contribution, if this is a reply.</param>
    public PostContribution(
        string threadId,
        ParticipantId authorId,
        string content,
        ContributionId? parentContributionId = null)
    {
        ThreadId = threadId ?? throw new ArgumentNullException(nameof(threadId));
        AuthorId = authorId ?? throw new ArgumentNullException(nameof(authorId));
        Content = content ?? throw new ArgumentNullException(nameof(content));
        ParentContributionId = parentContributionId;
    }
}

/// <summary>
/// Command handler for posting contributions to specific discourse threads.
/// </summary>
public static class PostContributionHandler
{
    /// <summary>
    /// Handles the post contribution to thread command.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="discourseThreadRepository">The discourse thread repository.</param>
    /// <param name="contributionRepository">The contribution repository.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the created contribution.</returns>
    public static async Task<ContributionId> Handle(
        PostContribution command,
        IDiscourseThreadRepository discourseThreadRepository,
        IContributionRepository contributionRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var threadId = DiscourseThreadId.Create(command.ThreadId);
        var thread = await discourseThreadRepository.GetByIdOrThrowAsync(threadId, cancellationToken).ConfigureAwait(false);

        var contribution = Contribution.Create(threadId, command.AuthorId, ContributionContent.Create(command.Content), command.ParentContributionId);
        contributionRepository.Stage(contribution);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return contribution.Id;
    }
}
