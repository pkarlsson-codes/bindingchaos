using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.SignalAwareness.Domain.Signals.Events;
using Microsoft.CodeAnalysis;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Aggregate root representing a community observation that may be relevant for coordination.
/// </summary>
public sealed class Signal : AggregateRoot<SignalId>
{
    private readonly List<SignalAmplification> _amplifications = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    /// <summary>
    /// Initializes a new instance of the Signal class.
    /// </summary>
    /// <param name="id">The unique identifier for this signal.</param>
    /// <param name="content">The content of this signal.</param>
    /// <param name="originatorId">The originator of this signal.</param>
    /// <param name="location">The optional geographic location where this signal occurred.</param>
    /// <param name="tags">Optional tags associated with the signal.</param>
    /// <param name="attachments">Optional attachments associated with the signal.</param>
    private Signal(SignalId id, SignalContent content,
        ParticipantId originatorId, Coordinates? location, string[] tags, AttachmentSpec[] attachments)
    {
        RegisterInvariants();
        ApplyChange(new SignalCaptured(
            id.Value,
            Version,
            content.Title,
            content.Description,
            originatorId.Value,
            location?.Latitude,
            location?.Longitude,
            tags,
            [.. attachments.Select(a => new SignalCaptured.AttachmentRecord(SignalAttachmentId.Generate().Value, a.DocumentId, a.Caption))]));
    }

    private Signal()
    {
        RegisterInvariants();
    }
#pragma warning restore CS8618

    /// <summary>
    /// Gets the participant who originated the signal.
    /// </summary>
    public ParticipantId OriginatorId { get; private set; }

    /// <summary>
    /// Gets the current content of the signal.
    /// </summary>
    internal SignalContent Content { get; private set; }

    /// <summary>
    /// Gets the current status of the signal.
    /// </summary>
    internal SignalStatus Status { get; private set; }

    /// <summary>
    /// Gets the subset of active (non-attenuated) amplifications.
    /// </summary>
    internal IReadOnlyCollection<SignalAmplification> ActiveAmplifications => _amplifications.Where(a => a.IsActive).ToList().AsReadOnly();

    /// <summary>
    /// Amplifies the signal by a participant.
    /// </summary>
    /// <param name="amplifierId">The participant amplifying the signal.</param>
    /// <param name="reason">The reason for amplification.</param>
    /// <param name="commentary">Optional commentary about the amplification.</param>
    public void Amplify(ParticipantId amplifierId, AmplificationReason reason, string? commentary = null)
    {
        ArgumentNullException.ThrowIfNull(amplifierId);
        if (Status != SignalStatus.Active)
        {
            throw new BusinessRuleViolationException("Cannot amplify non-active signal");
        }

        if (amplifierId.Equals(OriginatorId))
        {
            throw new BusinessRuleViolationException("Signal originator cannot amplify their own signal");
        }

        if (ActiveAmplifications.Any(a => a.AmplifierId.Equals(amplifierId)))
        {
            throw new BusinessRuleViolationException("Participant has already amplified this signal");
        }

        var amplificationId = SignalAmplificationId.Generate();
        ApplyChange(new SignalAmplified(
            Id.Value,
            Version,
            amplificationId.Value,
            amplifierId.Value,
            reason.Value,
            commentary));
    }

    /// <summary>
    /// Attenuates an amplification by a participant.
    /// </summary>
    /// <param name="amplifierId">The participant attenuating their amplification.</param>
    public void Attenuate(ParticipantId amplifierId)
    {
        ArgumentNullException.ThrowIfNull(amplifierId);
        if (Status != SignalStatus.Active)
        {
            throw new BusinessRuleViolationException("Cannot attenuate amplification from non-active signal");
        }

        var amplification = _amplifications.FirstOrDefault(a => a.AmplifierId.Equals(amplifierId) && a.IsActive)
            ?? throw new BusinessRuleViolationException("No active amplification found for this participant");

        ApplyChange(new SignalAmplificationAttenuated(Id.Value, Version, amplification.Id.Value, amplifierId.Value));
    }

    /// <summary>
    /// Creates a new signal.
    /// </summary>
    /// <param name="content">The content of the signal.</param>
    /// <param name="originatorId">The originator of the signal.</param>
    /// <param name="location">The optional geographic location where this signal occurred.</param>
    /// <param name="tags">Optional tags associated with the signal.</param>
    /// <param name="attachments">Optional attachments associated with the signal.</param>
    /// <returns>A new signal instance.</returns>
    internal static Signal Capture(SignalContent content, ParticipantId originatorId,
        Coordinates? location, string[] tags, AttachmentSpec[] attachments)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(originatorId);
        ArgumentNullException.ThrowIfNull(tags);
        ArgumentNullException.ThrowIfNull(attachments);

        return new Signal(SignalId.Generate(), content, originatorId, location, tags, attachments);
    }

    /// <summary>
    /// Adds an attachment to the signal.
    /// </summary>
    /// <param name="attachment">The attachment to add, including its document ID and caption. Cannot be <see langword="null"/>.</param>
    /// <exception cref="BusinessRuleViolationException">Thrown if the signal's status is not <see cref="SignalStatus.Active"/>.</exception>
    internal void AddAttachment(AttachmentSpec attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        if (Status != SignalStatus.Active)
        {
            throw new BusinessRuleViolationException("Cannot add attachment to non-active signal");
        }

        ApplyChange(new AttachmentAdded(Id.Value, Version, SignalAttachmentId.Generate().Value, attachment.DocumentId, attachment.Caption));
    }

    /// <summary>
    /// Updates the content of the signal.
    /// </summary>
    /// <param name="newContent">The new signal content.</param>
    internal void UpdateContent(SignalContent newContent)
    {
        ArgumentNullException.ThrowIfNull(newContent);
        if (Status != SignalStatus.Active)
        {
            throw new BusinessRuleViolationException("Cannot update content of non-active signal");
        }

        ApplyChange(new SignalContentUpdated(Id.Value, Version, newContent.Title, newContent.Description));
    }

    /// <summary>
    /// Adds the specified evidence to the current instance.
    /// </summary>
    /// <param name="evidence">The evidence to add. This parameter cannot be null.</param>
    internal void AddEvidence(EvidenceSpec evidence)
    {
        ArgumentNullException.ThrowIfNull(evidence);
        if (Status != SignalStatus.Active)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Applies a domain event to mutate aggregate state.
    /// </summary>
    /// <param name="domainEvent">The domain event to apply.</param>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        switch (domainEvent)
        {
            case SignalCaptured e: Apply(e); break;
            case SignalStatusChanged e: Apply(e); break;
            case SignalContentUpdated e: Apply(e); break;
            case SignalAmplified e: Apply(e); break;
            case SignalAmplificationAttenuated e: Apply(e); break;
            case AttachmentAdded e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    #region Invariants

    /// <summary>
    /// Invariant: content must include a title and description.
    /// </summary>
    private void ContentMustHaveTitleAndDescription()
    {
        if (Content == null)
        {
            throw new InvariantViolationException("Signal content cannot be null");
        }

        if (string.IsNullOrWhiteSpace(Content.Title))
        {
            throw new InvariantViolationException("Signal title cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(Content.Description))
        {
            throw new InvariantViolationException("Signal description cannot be empty");
        }
    }

    /// <summary>
    /// Invariant: the signal must have a non-null originator.
    /// </summary>
    private void OriginatorMustBeSpecified()
    {
        if (OriginatorId == null)
        {
            throw new InvariantViolationException("Signal originator cannot be null");
        }
    }

    /// <summary>
    /// Invariant: the signal status must be defined.
    /// </summary>
    private void StatusMustBeDefined()
    {
        if (Status == null)
        {
            throw new InvariantViolationException("Signal status cannot be null");
        }
    }

    /// <summary>
    /// Invariant: each participant can have at most one active amplification.
    /// </summary>
    private void ParticipantsMustHaveUniqueActiveAmplifications()
    {
        var activeAmplifications = _amplifications.Where(a => a.IsActive).ToList();
        var participantIds = activeAmplifications.Select(a => a.AmplifierId.Value).ToList();

        if (participantIds.Count != participantIds.Distinct().Count())
        {
            throw new InvariantViolationException("Each participant can have at most one active amplification");
        }
    }

    /// <summary>
    /// Invariant: the originator cannot amplify their own signal.
    /// </summary>
    private void OriginatorCannotAmplifyOwnSignal()
    {
        var activeAmplifications = _amplifications.Where(a => a.IsActive).ToList();

        if (activeAmplifications.Any(a => a.AmplifierId.Equals(OriginatorId.Value)))
        {
            throw new InvariantViolationException("Signal originator cannot amplify their own signal");
        }
    }

    #endregion

    private void RegisterInvariants()
    {
        AddInvariants(
            ContentMustHaveTitleAndDescription,
            OriginatorMustBeSpecified,
            StatusMustBeDefined,
            ParticipantsMustHaveUniqueActiveAmplifications,
            OriginatorCannotAmplifyOwnSignal);
    }

    private void Apply(SignalAmplificationAttenuated e)
    {
        var amplificationId = SignalAmplificationId.Create(e.AmplificationId);
        var amplification = _amplifications.Single(a => a.Id.Equals(amplificationId));
        amplification.Attenuate();
    }

    private void Apply(SignalCaptured @event)
    {
        Id = SignalId.Create(@event.AggregateId);
        Content = SignalContent.Create(@event.Title, @event.Description);
        OriginatorId = ParticipantId.Create(@event.OriginatorId);
        Status = SignalStatus.Active;
    }

    private void Apply(SignalStatusChanged @event)
    {
        Status = SignalStatus.FromValue(@event.NewStatus);
    }

    private void Apply(SignalContentUpdated @event)
    {
        Content = SignalContent.Create(@event.Title, @event.Description);
    }

    private void Apply(SignalAmplified @event)
    {
        var amplification = SignalAmplification.Create(
            SignalAmplificationId.Create(@event.AmplificationId),
            ParticipantId.Create(@event.AmplifierId));

        _amplifications.Add(amplification);
    }

#pragma warning disable CA1822
    private void Apply(AttachmentAdded e)
    {
        _ = e;
    }
#pragma warning restore CA1822
}
