using BindingChaos.CommunityDiscourse.Domain.Contributions.Events;
using BindingChaos.CommunityDiscourse.Domain.DiscourseThreads;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.CommunityDiscourse.Domain.Contributions;

/// <summary>
/// Entity representing an individual contribution within a discourse thread.
/// </summary>
public sealed class Contribution : AggregateRoot<ContributionId>
{
    /// <summary>
    /// Initializes a new instance of the Contribution class.
    /// </summary>
    /// <param name="threadId">The ID of the thread this contribution belongs to.</param>
    /// <param name="authorId">The ID of the participant who authored this contribution.</param>
    /// <param name="content">The content of the contribution.</param>
    /// <param name="parentContributionId">The ID of the parent contribution, if this is a reply.</param>
    private Contribution(
        DiscourseThreadId threadId,
        ParticipantId authorId,
        ContributionContent content,
        ContributionId? parentContributionId = null)
    {
        ApplyChange(new ContributionAdded(ContributionId.Generate().Value, 1, threadId.Value, authorId.Value, content.Value, parentContributionId?.Value));
    }

    /// <summary>
    /// Gets the ID of the participant who authored this contribution.
    /// </summary>
    public ParticipantId AuthorId { get; private set; } = null!;

    /// <summary>
    /// Gets the content of the contribution.
    /// </summary>
    public ContributionContent Content { get; private set; } = null!;

    /// <summary>
    /// Gets the ID of the thread this contribution belongs to.
    /// </summary>
    public DiscourseThreadId ThreadId { get; private set; } = null!;

    /// <summary>
    /// Gets the ID of the parent contribution, if this is a reply.
    /// </summary>
    public ContributionId? ParentContributionId { get; private set; }

    /// <summary>
    /// Gets the current status of the contribution.
    /// </summary>
    public ContributionStatus Status { get; private set; } = null!;

    /// <summary>
    /// Gets the timestamp when the contribution was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="Contribution"/> class with the specified thread, author, content, and optional
    /// parent contribution.
    /// </summary>
    /// <param name="threadId">The unique identifier of the thread this contribution belongs to.</param>
    /// <param name="authorId">The unique identifier of the participant who authored the contribution.</param>
    /// <param name="content">The content of the contribution.</param>
    /// <param name="parentContributionId">The optional identifier of the parent contribution, if this contribution is a reply. Defaults to <see
    /// langword="null"/> if not specified.</param>
    /// <returns>A new <see cref="Contribution"/> instance initialized with the specified parameters.</returns>
    public static Contribution Create(
        DiscourseThreadId threadId,
        ParticipantId authorId,
        ContributionContent content,
        ContributionId? parentContributionId = null)
    {
        return new Contribution(threadId, authorId, content, parentContributionId);
    }

    /// <summary>
    /// Applies the specified domain event to update the state of the aggregate.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply. Must not be <see langword="null"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown if the type of <paramref name="domainEvent"/> is not recognized.</exception>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        switch (domainEvent)
        {
            case ContributionAdded e:
                Apply(e);
                break;
            default:
                throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(ContributionAdded e)
    {
        Id = ContributionId.Create(e.AggregateId);
        ThreadId = DiscourseThreadId.Create(e.ThreadId);
        CreatedAt = e.OccurredAt;
        AuthorId = ParticipantId.Create(e.AuthorId);
        ParentContributionId = e.ParentContributionId != null ? ContributionId.Create(e.ParentContributionId) : null;
        Content = ContributionContent.Create(e.Content);
        Status = ContributionStatus.Published;
    }
}