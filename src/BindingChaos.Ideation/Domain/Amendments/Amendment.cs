using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;

namespace BindingChaos.Ideation.Domain.Amendments;

/// <summary>
/// Represents a structured proposal to modify a published idea.
/// Participants may support or oppose until the amendment is resolved.
/// </summary>
public sealed class Amendment : AggregateRoot<AmendmentId>
{
    private readonly List<Supporter> _supporters = [];
    private readonly List<Opponent> _opponents = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="Amendment"/> aggregate.
    /// Use <see cref="Propose"/> to create a new instance.
    /// </summary>
    /// <param name="id">The unique identifier for this amendment.</param>
    /// <param name="targetIdeaId">The target idea ID.</param>
    /// <param name="targetVersionNumber">The target idea version number.</param>
    /// <param name="creatorId">The creator of this amendment.</param>
    /// <param name="proposedTitle">The proposed title.</param>
    /// <param name="proposedBody">The proposed body.</param>
    /// <param name="amendmentTitle">The amendment title.</param>
    /// <param name="amendmentDescription">The amendment description.</param>
    private Amendment(AmendmentId id, IdeaId targetIdeaId, int targetVersionNumber,
        ParticipantId creatorId, string proposedTitle, string proposedBody, string amendmentTitle, string amendmentDescription)
    {
        RegisterInvariants();

        ApplyChange(new Events.AmendmentProposed(
            id.Value,
            targetIdeaId.Value,
            targetVersionNumber,
            creatorId.Value,
            proposedTitle,
            proposedBody,
            amendmentTitle,
            amendmentDescription));
    }

    private Amendment()
    {
        RegisterInvariants();
    }
#pragma warning restore CS8618

    /// <summary>
    /// Gets the target idea being amended.
    /// </summary>
    public IdeaId TargetIdeaId { get; private set; }

    /// <summary>
    /// Gets the target version number of the idea that this amendment applies to.
    /// </summary>
    public int TargetVersionNumber { get; private set; }

    /// <summary>
    /// Gets the creator of this amendment.
    /// </summary>
    public ParticipantId CreatorId { get; private set; }

    /// <summary>
    /// Gets the current status of this amendment.
    /// </summary>
    public AmendmentStatus? Status { get; private set; }

    /// <summary>
    /// Gets the proposed title.
    /// </summary>
    public string ProposedTitle { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the proposed body.
    /// </summary>
    public string ProposedBody { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the amendment title.
    /// </summary>
    public string AmendmentTitle { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the amendment description.
    /// </summary>
    public string AmendmentDescription { get; private set; } = string.Empty;

    /// <summary>
    /// Gets when the amendment was accepted, if applicable.
    /// </summary>
    public DateTimeOffset? AcceptedAt { get; private set; }

    /// <summary>
    /// Gets when the amendment was rejected, if applicable.
    /// </summary>
    public DateTimeOffset? RejectedAt { get; private set; }

    /// <summary>
    /// The list of participants who support this amendment.
    /// </summary>
    public IReadOnlyList<Supporter> Supporters => _supporters.AsReadOnly();

    /// <summary>
    /// The list of participants who oppose this amendment.
    /// </summary>
    public IReadOnlyList<Opponent> Opponents => _opponents.AsReadOnly();

    /// <summary>
    /// Proposes a new amendment to an existing idea.
    /// </summary>
    /// <param name="targetIdeaId">The ID of the idea being amended.</param>
    /// <param name="targetVersionNumber">The version number of the idea being amended.</param>
    /// <param name="creatorId">The ID of the participant proposing the amendment.</param>
    /// <param name="proposedTitle">The proposed title for the amendment.</param>
    /// <param name="proposedBody">The proposed body for the amendment.</param>
    /// <param name="amendmentTitle">The title of the amendment.</param>
    /// <param name="amendmentDescription">The description of the amendment.</param>
    /// <returns>A new <see cref="Amendment"/> instance representing the proposed amendment.</returns>
    /// <exception cref="ArgumentException">Thrown if the title or body is empty, or if the target version number is less than 1.</exception>
    public static Amendment Propose(IdeaId targetIdeaId, int targetVersionNumber,
        ParticipantId creatorId, string proposedTitle, string proposedBody, string amendmentTitle, string amendmentDescription)
    {
        if (string.IsNullOrWhiteSpace(proposedTitle))
        {
            throw new ArgumentException("Title cannot be empty", nameof(proposedTitle));
        }

        if (string.IsNullOrWhiteSpace(proposedBody))
        {
            throw new ArgumentException("Body cannot be empty", nameof(proposedBody));
        }

        if (string.IsNullOrWhiteSpace(amendmentTitle))
        {
            throw new ArgumentException("Amendment title cannot be empty", nameof(amendmentTitle));
        }

        if (string.IsNullOrWhiteSpace(amendmentDescription))
        {
            throw new ArgumentException("Amendment description cannot be empty", nameof(amendmentDescription));
        }

        if (targetVersionNumber < 1)
        {
            throw new ArgumentException("Target version must be at least 1", nameof(targetVersionNumber));
        }

        var id = AmendmentId.Generate();
        return new Amendment(id, targetIdeaId, targetVersionNumber, creatorId, proposedTitle, proposedBody, amendmentTitle, amendmentDescription);
    }

    /// <summary>
    /// Adds a participant's support for this amendment.
    /// </summary>
    /// <param name="supporter">The supporter with their reason for supporting the amendment.</param>
    public void AddSupport(Supporter supporter)
    {
        if (CreatorId == supporter.ParticipantId)
        {
            throw new BusinessRuleViolationException("Participant cannot support their own amendment.");
        }

        if (Status != AmendmentStatus.Open)
        {
            throw new BusinessRuleViolationException("Cannot support an amendment that is not open");
        }

        if (_supporters.Any(s => s.ParticipantId == supporter.ParticipantId))
        {
            throw new BusinessRuleViolationException("Participant has already supported this amendment");
        }

        if (_opponents.Any(o => o.ParticipantId == supporter.ParticipantId))
        {
            throw new BusinessRuleViolationException("Participant cannot both support and oppose the same amendment");
        }

        ApplyChange(new Events.AmendmentSupportAdded(Id.Value, supporter.ParticipantId.Value, supporter.Reason));
    }

    /// <summary>
    /// Withdraws a participant's support for this amendment.
    /// </summary>
    /// <param name="participantId">The participant withdrawing support.</param>
    public void WithdrawSupport(ParticipantId participantId)
    {
        if (Status != AmendmentStatus.Open)
        {
            throw new BusinessRuleViolationException("Cannot withdraw support from an amendment that is not open");
        }

        if (!_supporters.Any(s => s.ParticipantId == participantId))
        {
            throw new BusinessRuleViolationException("Participant has not supported this amendment");
        }

        ApplyChange(new Events.AmendmentSupportWithdrawn(Id.Value, participantId.Value));
    }

    /// <summary>
    /// Adds a participant's opposition to this amendment.
    /// </summary>
    /// <param name="opponent">The opponent with their reason for opposing the amendment.</param>
    public void AddOpposition(Opponent opponent)
    {
        if (CreatorId == opponent.ParticipantId)
        {
            throw new BusinessRuleViolationException("Participant cannot oppose their own amendment.");
        }

        if (Status != AmendmentStatus.Open)
        {
            throw new BusinessRuleViolationException("Cannot oppose an amendment that is not open");
        }

        if (_opponents.Any(o => o.ParticipantId == opponent.ParticipantId))
        {
            throw new BusinessRuleViolationException("Participant has already opposed this amendment");
        }

        if (_supporters.Any(s => s.ParticipantId == opponent.ParticipantId))
        {
            throw new BusinessRuleViolationException("Participant cannot both support and oppose the same amendment");
        }

        ApplyChange(new Events.AmendmentOppositionAdded(Id.Value, opponent.ParticipantId.Value, opponent.Reason));
    }

    /// <summary>
    /// Withdraws a participant's opposition to this amendment.
    /// </summary>
    /// <param name="participantId">The participant withdrawing opposition.</param>
    public void WithdrawOpposition(ParticipantId participantId)
    {
        if (Status != AmendmentStatus.Open)
        {
            throw new BusinessRuleViolationException("Cannot withdraw opposition from an amendment that is not open");
        }

        if (!_opponents.Any(o => o.ParticipantId == participantId))
        {
            throw new BusinessRuleViolationException("Participant has not opposed this amendment");
        }

        ApplyChange(new Events.AmendmentOppositionWithdrawn(Id.Value, participantId.Value));
    }

    /// <summary>
    /// Accepts this amendment, resolving it and indicating the new idea version to be created.
    /// </summary>
    public void Accept()
    {
        if (Status != AmendmentStatus.Open)
        {
            throw new BusinessRuleViolationException("Only open amendments can be accepted");
        }

        ApplyChange(new Events.AmendmentAccepted(Id.Value, TargetIdeaId.Value));
    }

    /// <summary>
    /// Rejects this amendment, resolving it without changes to the target idea.
    /// </summary>
    public void Reject()
    {
        if (Status != AmendmentStatus.Open)
        {
            throw new BusinessRuleViolationException("Only open amendments can be rejected");
        }

        ApplyChange(new Events.AmendmentRejected(Id.Value, TargetIdeaId.Value));
    }

    /// <summary>
    /// Marks this amendment as outdated due to the target idea advancing versions.
    /// </summary>
    /// <param name="newCurrentVersionNumber">The new current version of the target idea.</param>
    public void MarkOutdated(int newCurrentVersionNumber)
    {
        if (Status != AmendmentStatus.Open)
        {
            return;
        }

        if (newCurrentVersionNumber <= TargetVersionNumber)
        {
            return;
        }

        ApplyChange(new Events.AmendmentMarkedOutdated(Id.Value, TargetIdeaId.Value, TargetVersionNumber, newCurrentVersionNumber));
    }

    /// <summary>
    /// Marks an approved amendment as outdated when it fails to apply due to version mismatch.
    /// This is a compensating action when saga-driven approval encounters a race condition.
    /// </summary>
    /// <param name="newCurrentVersionNumber">The actual current version of the target idea.</param>
    public void MarkOutdatedAfterApplicationFailure(int newCurrentVersionNumber)
    {
        if (Status != AmendmentStatus.Approved)
        {
            throw new BusinessRuleViolationException(
                $"Only approved amendments can be marked outdated after application failure. Current status: {Status?.DisplayName ?? "Unknown"}");
        }

        if (newCurrentVersionNumber <= TargetVersionNumber)
        {
            throw new BusinessRuleViolationException(
                $"Cannot mark amendment as outdated: new version {newCurrentVersionNumber} is not greater than target version {TargetVersionNumber}");
        }

        ApplyChange(new Events.AmendmentMarkedOutdated(Id.Value, TargetIdeaId.Value, TargetVersionNumber, newCurrentVersionNumber));
    }

    /// <summary>
    /// Retargets an outdated amendment to the current idea version and optionally updates proposed content.
    /// </summary>
    /// <param name="actorId">The participant performing the retargeting (must be the creator).</param>
    /// <param name="newTargetVersionNumber">The new idea version to target.</param>
    /// <param name="newTitle">Optional new proposed title.</param>
    /// <param name="newBody">Optional new proposed body.</param>
    /// <param name="newAmendmentTitle">Optional new amendment title.</param>
    /// <param name="newAmendmentDescription">Optional new amendment description.</param>
    public void Retarget(ParticipantId actorId, int newTargetVersionNumber, string? newTitle = null, string? newBody = null, string? newAmendmentTitle = null, string? newAmendmentDescription = null)
    {
        if (Status != AmendmentStatus.Outdated)
        {
            throw new BusinessRuleViolationException("Only outdated amendments can be retargeted");
        }

        if (!CreatorId.Equals(actorId))
        {
            throw new BusinessRuleViolationException("Only the creator can retarget an amendment");
        }

        if (newTargetVersionNumber <= TargetVersionNumber)
        {
            throw new ArgumentException("New target version must be greater than current target version", nameof(newTargetVersionNumber));
        }

        if (newTitle is not null && string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("Title cannot be empty when provided", nameof(newTitle));
        }

        if (newBody is not null && string.IsNullOrWhiteSpace(newBody))
        {
            throw new ArgumentException("Body cannot be empty when provided", nameof(newBody));
        }

        if (newAmendmentTitle is not null && string.IsNullOrWhiteSpace(newAmendmentTitle))
        {
            throw new ArgumentException("Amendment title cannot be empty when provided", nameof(newAmendmentTitle));
        }

        if (newAmendmentDescription is not null && string.IsNullOrWhiteSpace(newAmendmentDescription))
        {
            throw new ArgumentException("Amendment description cannot be empty when provided", nameof(newAmendmentDescription));
        }

        ApplyChange(new Events.AmendmentRetargeted(
            Id.Value,
            TargetIdeaId.Value,
            TargetVersionNumber,
            newTargetVersionNumber,
            newTitle,
            newBody,
            newAmendmentTitle,
            newAmendmentDescription));
    }

    /// <summary>
    /// Withdraws this amendment. Only the creator may withdraw while open.
    /// </summary>
    /// <param name="actorId">The participant attempting to withdraw the amendment.</param>
    public void Withdraw(ParticipantId actorId)
    {
        if (Status != AmendmentStatus.Open)
        {
            throw new BusinessRuleViolationException("Only open amendments can be withdrawn");
        }

        if (!CreatorId.Equals(actorId))
        {
            throw new BusinessRuleViolationException("Only the creator can withdraw an amendment");
        }

        ApplyChange(new Events.AmendmentWithdrawn(Id.Value, actorId.Value));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        switch (domainEvent)
        {
            case Events.AmendmentProposed e: Apply(e); break;
            case Events.AmendmentSupportAdded e: Apply(e); break;
            case Events.AmendmentSupportWithdrawn e: Apply(e); break;
            case Events.AmendmentOppositionAdded e: Apply(e); break;
            case Events.AmendmentOppositionWithdrawn e: Apply(e); break;
            case Events.AmendmentAccepted e: Apply(e); break;
            case Events.AmendmentRejected e: Apply(e); break;
            case Events.AmendmentMarkedOutdated e: Apply(e); break;
            case Events.AmendmentRetargeted e: Apply(e); break;
            case Events.AmendmentWithdrawn e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    private void RegisterInvariants()
    {
        AddInvariants(
            CreatorMustBeSpecified,
            StatusMustBeDefined,
            TitleMustBeNonEmpty,
            BodyMustBeNonEmpty,
            AmendmentTitleMustBeNonEmpty,
            AmendmentDescriptionMustBeNonEmpty,
            TargetVersionMustBeValid,
            ParticipantsCannotSupportAndOppose,
            AcceptedTimestampOnlyWhenApproved,
            RejectedTimestampOnlyWhenRejected);
    }

    private void Apply(Events.AmendmentProposed evt)
    {
        Id = AmendmentId.Create(evt.AggregateId);
        TargetIdeaId = IdeaId.Create(evt.TargetIdeaId);
        TargetVersionNumber = evt.TargetVersionNumber;
        CreatorId = ParticipantId.Create(evt.CreatorId);
        ProposedTitle = evt.ProposedTitle;
        ProposedBody = evt.ProposedBody;
        AmendmentTitle = evt.AmendmentTitle;
        AmendmentDescription = evt.AmendmentDescription;
        Status = AmendmentStatus.Open;
    }

    private void Apply(Events.AmendmentSupportAdded evt)
    {
        var participantId = ParticipantId.Create(evt.SupporterId);
        _supporters.Add(new Supporter(participantId, evt.Reason));
    }

    private void Apply(Events.AmendmentSupportWithdrawn evt)
    {
        var participantId = ParticipantId.Create(evt.SupporterId);
        _supporters.RemoveAll(s => s.ParticipantId == participantId);
    }

    private void Apply(Events.AmendmentOppositionAdded evt)
    {
        var participantId = ParticipantId.Create(evt.OpponentId);
        _opponents.Add(new Opponent(participantId, evt.Reason));
    }

    private void Apply(Events.AmendmentOppositionWithdrawn evt)
    {
        var participantId = ParticipantId.Create(evt.OpponentId);
        _opponents.RemoveAll(o => o.ParticipantId == participantId);
    }

    private void Apply(Events.AmendmentAccepted evt)
    {
        Status = AmendmentStatus.Approved;
        AcceptedAt = evt.OccurredAt;
    }

    private void Apply(Events.AmendmentRejected evt)
    {
        Status = AmendmentStatus.Rejected;
        RejectedAt = evt.OccurredAt;
    }

    private void Apply(Events.AmendmentMarkedOutdated evt)
    {
        Status = AmendmentStatus.Outdated;
    }

    private void Apply(Events.AmendmentRetargeted evt)
    {
        TargetVersionNumber = evt.NewTargetVersionNumber;
        if (!string.IsNullOrWhiteSpace(evt.NewTitle))
        {
            ProposedTitle = evt.NewTitle!;
        }

        if (!string.IsNullOrWhiteSpace(evt.NewBody))
        {
            ProposedBody = evt.NewBody!;
        }

        if (!string.IsNullOrWhiteSpace(evt.NewAmendmentTitle))
        {
            AmendmentTitle = evt.NewAmendmentTitle!;
        }

        if (!string.IsNullOrWhiteSpace(evt.NewAmendmentDescription))
        {
            AmendmentDescription = evt.NewAmendmentDescription!;
        }

        Status = AmendmentStatus.Open;
    }

    private void Apply(Events.AmendmentWithdrawn evt)
    {
        Status = AmendmentStatus.Withdrawn;
    }

    #region Invariants

    private void CreatorMustBeSpecified()
    {
        if (CreatorId == null)
        {
            throw new InvariantViolationException("Amendment creator cannot be null");
        }
    }

    private void StatusMustBeDefined()
    {
        if (Status == null)
        {
            throw new InvariantViolationException("Amendment status cannot be null");
        }
    }

    private void TitleMustBeNonEmpty()
    {
        if (string.IsNullOrWhiteSpace(ProposedTitle))
        {
            throw new InvariantViolationException("Amendment proposed title cannot be empty");
        }
    }

    private void BodyMustBeNonEmpty()
    {
        if (string.IsNullOrWhiteSpace(ProposedBody))
        {
            throw new InvariantViolationException("Amendment proposed body cannot be empty");
        }
    }

    private void AmendmentTitleMustBeNonEmpty()
    {
        if (string.IsNullOrWhiteSpace(AmendmentTitle))
        {
            throw new InvariantViolationException("Amendment title cannot be empty");
        }
    }

    private void AmendmentDescriptionMustBeNonEmpty()
    {
        if (string.IsNullOrWhiteSpace(AmendmentDescription))
        {
            throw new InvariantViolationException("Amendment description cannot be empty");
        }
    }

    private void TargetVersionMustBeValid()
    {
        if (TargetVersionNumber < 1)
        {
            throw new InvariantViolationException("Target version number must be at least 1");
        }
    }

    private void ParticipantsCannotSupportAndOppose()
    {
        var supporterIds = _supporters.Select(s => s.ParticipantId).ToHashSet();
        var opponentIds = _opponents.Select(o => o.ParticipantId).ToHashSet();
        if (supporterIds.Overlaps(opponentIds))
        {
            throw new InvariantViolationException("A participant cannot both support and oppose the same amendment");
        }
    }

    private void AcceptedTimestampOnlyWhenApproved()
    {
        if (AcceptedAt.HasValue && Status != AmendmentStatus.Approved)
        {
            throw new InvariantViolationException("AcceptedOn can only be set when status is Approved");
        }
    }

    private void RejectedTimestampOnlyWhenRejected()
    {
        if (RejectedAt.HasValue && Status != AmendmentStatus.Rejected)
        {
            throw new InvariantViolationException("RejectedOn can only be set when status is Rejected");
        }
    }

    #endregion
}
