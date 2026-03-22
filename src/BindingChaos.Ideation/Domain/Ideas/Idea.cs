using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Ideas.Events;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Represents a publicly visible and versioned proposal derived from a participant's submitted draft.
/// </summary>
public sealed class Idea : AggregateRoot<IdeaId>
{
    private readonly List<string> _signalReferences = [];

    private readonly List<string> _tags = [];

    private readonly List<IdeaVersion> _versionHistory = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    private Idea(IdeaId id, SocietyId societyContext, ParticipantId creatorId, string title, string body, IEnumerable<string> signalReferences, IEnumerable<string> tags)
        : this()
    {
        ApplyChange(new IdeaAuthored(id.Value, creatorId.Value, societyContext.Value, title, body, [.. signalReferences],
            [.. tags], null));
    }

    private Idea(IdeaId id, SocietyId societyContext, ParticipantId creatorId, string title, string body,
        IEnumerable<string> signalReferences, IEnumerable<string> tags, IdeaId parentIdeaId)
        : this()
    {
        ApplyChange(new IdeaForked(id.Value, creatorId.Value, societyContext.Value, title, body, [.. signalReferences],
            [.. tags], parentIdeaId.Value));
    }

    private Idea()
    {
        RegisterInvariants();
    }
#pragma warning restore CS8618

    /// <summary>
    /// Gets the creator of this idea.
    /// </summary>
    public ParticipantId? CreatorId { get; private set; }

    /// <summary>
    /// Gets the current status of this idea.
    /// </summary>
    public IdeaStatus? Status { get; private set; }

    /// <summary>
    /// Gets the current version of this idea.
    /// </summary>
    public IdeaVersion CurrentVersion { get; private set; } = null!;

    /// <summary>
    /// Creates a new idea authored by a participant based on existing signal references.
    /// </summary>
    /// <param name="societyContext">The society context (governance jurisdiction) for this idea.</param>
    /// <param name="creatorId">The creator of this idea.</param>
    /// <param name="title">The title of the initial version.</param>
    /// <param name="body">The body content of the initial version.</param>
    /// <param name="signalReferences">The signal references for this idea.</param>
    /// <param name="tags">The tags for this idea.</param>
    /// <returns>A new idea instance.</returns>
    /// <exception cref="ArgumentException">Thrown if no signal references are provided.</exception>
    public static Idea Author(SocietyId societyContext, ParticipantId creatorId, string title, string body,
        string[] signalReferences, string[] tags)
    {
        if (signalReferences.Length == 0)
        {
            throw new BusinessRuleViolationException("At least one signal reference is required");
        }

        return new Idea(IdeaId.Generate(), societyContext, creatorId, title, body, signalReferences, tags);
    }

    /// <summary>
    /// Creates a new forked idea based on an existing parent idea.
    /// </summary>
    /// <param name="societyContext">The society context (governance jurisdiction) for this idea.</param>
    /// <param name="authorId">The identifier of the participant creating the forked idea.</param>
    /// <param name="title">The title of the new forked idea. Cannot be null or empty.</param>
    /// <param name="body">The body content of the new forked idea. Cannot be null or empty.</param>
    /// <param name="parentIdeaId">The identifier of the parent idea from which this idea is forked.</param>
    /// <param name="sourceSignalIds">A collection of signal identifiers associated with the new forked idea. Can be empty but cannot be null.</param>
    /// <param name="tags">A collection of tags associated with the new forked idea. Can be empty but cannot be null.</param>
    /// <returns>A new <see cref="Idea"/> instance representing the forked idea.</returns>
    public static Idea CreateFork(SocietyId societyContext, ParticipantId authorId, string title, string body,
        IdeaId parentIdeaId, string[] sourceSignalIds, string[] tags)
    {
        return new Idea(IdeaId.Generate(), societyContext, authorId, title, body, sourceSignalIds, tags, parentIdeaId);
    }

    /// <summary>
    /// Adds a new requirement to the project using the specified requirement specification.
    /// </summary>
    /// <param name="spec">The specification that defines the properties and behavior of the requirement to add. Cannot be null.</param>
    /// <param name="addedBy">Participant that added the requirement.</param>
    /// <returns>The id of the added requirement.</returns>
    public RequirementId AddRequirement(RequirementSpec spec, ParticipantId addedBy)
    {
        ArgumentNullException.ThrowIfNull(spec);

        var requirementId = RequirementId.Generate();
        ApplyChange(new RequirementAdded(Id.Value, requirementId.Value, spec.Label, spec.Quantity, spec.Unit, spec.Type.Value, addedBy.Value));
        return requirementId;
    }

    /// <summary>
    /// Adds a tag to this idea.
    /// </summary>
    /// <param name="userId">The user adding the tag.</param>
    /// <param name="tagId">The tag ID to add.</param>
    public void AddTag(ParticipantId userId, string tagId)
    {
        if (_tags.Contains(tagId))
        {
            throw new BusinessRuleViolationException($"Tag `{tagId}` already exists on idea with id `{Id}`.");
        }

        ApplyChange(new TagAddedToIdea(Id.Value, userId, tagId));
    }

    /// <summary>
    /// Removes a tag from this idea.
    /// </summary>
    /// <param name="userId">The user removing the tag.</param>
    /// <param name="tagId">The tag ID to remove.</param>
    public void RemoveTag(ParticipantId userId, string tagId)
    {
        if (!_tags.Contains(tagId))
        {
            throw new BusinessRuleViolationException($"Tag '{tagId}' does not exist on idea with id `{Id}`.");
        }

        ApplyChange(new TagRemovedFromIdea(Id.Value, userId, tagId));
    }

    /// <summary>
    /// Applies an amendment to create a new version of this idea.
    /// </summary>
    /// <param name="amendmentId">The amendment ID.</param>
    /// <param name="newTitle">The new title.</param>
    /// <param name="newBody">The new body content.</param>
    public void Amend(AmendmentId amendmentId, string newTitle, string newBody)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new BusinessRuleViolationException("Title cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(newBody))
        {
            throw new BusinessRuleViolationException("Body cannot be empty");
        }

        var newVersionNumber = _versionHistory.Count + 1;
        ApplyChange(new IdeaAmended(Id.Value, amendmentId.Value, newVersionNumber, newTitle, newBody));
    }

    /// <summary>
    /// Applies a domain event to the aggregate, updating its state.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply.</param>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IdeaAuthored e: Apply(e); break;
            case IdeaForked e: Apply(e); break;
            case TagAddedToIdea e: Apply(e); break;
            case TagRemovedFromIdea e: Apply(e); break;
            case IdeaAmended e: Apply(e); break;
            case RequirementAdded e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent?.GetType().Name}");
        }
    }

    private void RegisterInvariants()
    {
        AddInvariants(
            CreatorMustBeSpecified,
            StatusMustBeDefined,
            MustHaveAtLeastOneSignalReference,
            MustHaveAtLeastOneVersion,
            CurrentVersionMustMatchLatestVersion,
            VersionHistoryMustHaveStrictlyIncreasingNumbers);
    }

    private void Apply(IdeaAuthored evt)
    {
        Id = IdeaId.Create(evt.AggregateId);
        CreatorId = ParticipantId.Create(evt.AuthorId);
        Status = IdeaStatus.Published;
        _signalReferences.AddRange(evt.SignalReferences);
        _tags.AddRange(evt.Tags);
        var initialVersion = IdeaVersion.CreateFromDraft(evt.Title, evt.Body);
        _versionHistory.Add(initialVersion);
        CurrentVersion = initialVersion;
    }

    private void Apply(IdeaForked evt)
    {
        Id = IdeaId.Create(evt.AggregateId);
        CreatorId = ParticipantId.Create(evt.AuthorId);
        Status = IdeaStatus.Published;
        _signalReferences.AddRange(evt.SignalReferences);
        _tags.AddRange(evt.Tags);
        var initialVersion = IdeaVersion.CreateFromDraft(evt.Title, evt.Body);
        _versionHistory.Add(initialVersion);
        CurrentVersion = initialVersion;
    }

    private void Apply(TagAddedToIdea evt)
    {
        _tags.Add(evt.TagId);
    }

    private void Apply(TagRemovedFromIdea evt)
    {
        _tags.Remove(evt.TagId);
    }

    private void Apply(IdeaAmended evt)
    {
        var newVersion = IdeaVersion.CreateFromAmendment(evt.NewVersionNumber, evt.NewTitle, evt.NewBody, AmendmentId.Create(evt.AmendmentId));
        _versionHistory.Add(newVersion);
        CurrentVersion = newVersion;
    }

#pragma warning disable CA1822
    private void Apply(RequirementAdded evt)
    {
        _ = evt;
    }
#pragma warning restore CA1822

    #region Invariants

    private void CreatorMustBeSpecified()
    {
        if (CreatorId == null)
        {
            throw new InvariantViolationException("Idea creator cannot be null");
        }
    }

    private void StatusMustBeDefined()
    {
        if (Status == null)
        {
            throw new InvariantViolationException("Idea status cannot be null");
        }
    }

    private void MustHaveAtLeastOneSignalReference()
    {
        if (_signalReferences.Count == 0)
        {
            throw new InvariantViolationException("Idea must have at least one signal reference");
        }
    }

    private void MustHaveAtLeastOneVersion()
    {
        if (_versionHistory.Count == 0)
        {
            throw new InvariantViolationException("Idea must have at least one version in history");
        }
    }

    private void CurrentVersionMustMatchLatestVersion()
    {
        if (CurrentVersion == null)
        {
            throw new InvariantViolationException("Current version cannot be null");
        }

        if (_versionHistory.Count == 0)
        {
            throw new InvariantViolationException("Version history cannot be empty");
        }

        var latestVersion = _versionHistory.Last();
        if (!CurrentVersion.Equals(latestVersion))
        {
            throw new InvariantViolationException("Current version must match the latest version in history");
        }
    }

    private void VersionHistoryMustHaveStrictlyIncreasingNumbers()
    {
        for (int i = 1; i < _versionHistory.Count; i++)
        {
            if (_versionHistory[i].VersionNumber <= _versionHistory[i - 1].VersionNumber)
            {
                throw new InvariantViolationException("Version history must have strictly increasing version numbers");
            }
        }
    }

    #endregion
}
