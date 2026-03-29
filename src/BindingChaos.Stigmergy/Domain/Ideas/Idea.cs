using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.Ideas.Events;

namespace BindingChaos.Stigmergy.Domain.Ideas;

/// <summary>
/// An Idea.
/// </summary>
public sealed class Idea : AggregateRoot<IdeaId>
{
    private IdeaStatus _status;
    private ParticipantId _authorId;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Idea() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    /// <summary>
    /// Drafts an <see cref="Idea"/>.
    /// </summary>
    /// <param name="authorId">Id of draft author.</param>
    /// <param name="title">Title of the draft.</param>
    /// <param name="description">Description of the draft.</param>
    /// <returns>The draft <see cref="Idea"/>.</returns>
    public static Idea Draft(ParticipantId authorId, string title, string description)
    {
        ArgumentNullException.ThrowIfNull(authorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        var idea = new Idea();
        var aggregateId = IdeaId.Generate();
        idea.ApplyChange(new IdeaDrafted(aggregateId.Value, authorId.Value, title, description));
        return idea;
    }

    /// <summary>
    /// Fork the idea.
    /// </summary>
    /// <param name="authorId">Id of author forking the idea.</param>
    /// <param name="title">Title of the forked idea.</param>
    /// <param name="description">Description of the forked idea.</param>
    /// <returns>The forked idea.</returns>
    public Idea Fork(ParticipantId authorId, string title, string description)
    {
        ArgumentNullException.ThrowIfNull(authorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        if (authorId != this._authorId && _status == IdeaStatus.Draft)
        {
            throw new ForbiddenException("Only the idea author can fork a draft.");
        }

        var idea = new Idea();
        var aggregateId = IdeaId.Generate();
        idea.ApplyChange(new IdeaForked(aggregateId.Value, Id.Value, authorId.Value, title, description));
        return idea;
    }

    /// <summary>
    /// Publishes the idea.
    /// </summary>
    /// <param name="actorId">The actor trying to publish the idea.</param>
    /// <exception cref="ForbiddenException">Throw if actor is not the idea author.</exception>
    public void Publish(ParticipantId actorId)
    {
        if (_authorId != actorId)
        {
            throw new ForbiddenException("Only an ideas author may publish it.");
        }

        if (_status != IdeaStatus.Draft)
        {
            throw new BusinessRuleViolationException("Can't publish already published Idea");
        }

        ApplyChange(new IdeaPublished(Id.Value, actorId.Value));
    }

    /// <summary>
    /// Updates a draft idea.
    /// </summary>
    /// <param name="actorId">Id of the user updating the draft.</param>
    /// <param name="title">Updated title.</param>
    /// <param name="description">Updated description.</param>
    /// <exception cref="ForbiddenException">Throw if actor is not the idea author.</exception>
    /// <exception cref="BusinessRuleViolationException">Throw if <see cref="Idea"/> is already published.</exception>
    public void Update(ParticipantId actorId, string title, string description)
    {
        if (actorId != _authorId)
        {
            throw new ForbiddenException("Only an ideas author may update it.");
        }

        if (_status == IdeaStatus.Published)
        {
            throw new BusinessRuleViolationException("Can't update a published Idea");
        }

        ApplyChange(new IdeaDraftUpdated(Id.Value, actorId.Value, title, description));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IdeaDrafted e: Apply(e); break;
            case IdeaForked e: Apply(e); break;
            case IdeaDraftUpdated e: Apply(e); break;
            case IdeaPublished e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(IdeaDrafted e)
    {
        Id = IdeaId.Create(e.AggregateId);
        _authorId = ParticipantId.Create(e.AuthorId);
        _status = IdeaStatus.Draft;
    }

#pragma warning disable CA1822
    private void Apply(IdeaForked e)
    {
        // Noop because there is no state to modify.
    }

    private void Apply(IdeaDraftUpdated e)
    {
        // Noop because there is no state to modify.
    }
#pragma warning restore CA1822

    private void Apply(IdeaPublished e)
    {
        _status = IdeaStatus.Published;
    }
}