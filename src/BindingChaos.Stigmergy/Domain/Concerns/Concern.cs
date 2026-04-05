using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.Concerns.Events;
using BindingChaos.Stigmergy.Domain.Signals;

namespace BindingChaos.Stigmergy.Domain.Concerns;

/// <summary>
/// A concern.
/// </summary>
public sealed class Concern : AggregateRoot<ConcernId>
{
    private readonly HashSet<ParticipantId> _affectedParticipants = [];

    private Concern()
    {
    }

    /// <summary>
    /// Raises a concern.
    /// </summary>
    /// <param name="actorId">Id of the actor raising the concern.</param>
    /// <param name="name">The name of the concern.</param>
    /// <param name="tags">Tags associated with the raised concern.</param>
    /// <param name="signalIds">Ids of <see cref="Signals"/> revealing the concern.</param>
    /// <returns>The raised concern.</returns>
    public static Concern Raise(
        ParticipantId actorId,
        string name,
        IReadOnlyList<string> tags,
        IReadOnlyList<SignalId> signalIds)
    {
        ArgumentNullException.ThrowIfNull(actorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (signalIds.Count == 0)
        {
            throw new BusinessRuleViolationException("Cannot raise a concern with no signals.");
        }

        var concern = new Concern();
        var concernId = ConcernId.Generate();
        IReadOnlyList<string> signalIdValues = [.. signalIds.Select(i => i.Value)];
        concern.ApplyChange(new ConcernRaised(concernId.Value, actorId.Value, name, tags, signalIdValues));
        return concern;
    }

    /// <summary>
    /// Indicates the affectedness of this concern for a participant.
    /// </summary>
    /// <param name="participantId">Id of the participant indicating the affectedness.</param>
    /// <exception cref="BusinessRuleViolationException">Thrown when the participant has already indicated affectedness for this concern.</exception>
    public void IndicateAffectedness(ParticipantId participantId)
    {
        ArgumentNullException.ThrowIfNull(participantId);

        if (_affectedParticipants.Contains(participantId))
        {
            throw new BusinessRuleViolationException("Participant has already indicated affectedness for this concern.");
        }

        ApplyChange(new AffectednessIndicated(Id.Value, participantId.Value));
    }

    /// <summary>
    /// Withdraws the affectedness indication of this concern for a participant.
    /// </summary>
    /// <param name="participantId">Id of the participant withdrawing the affectedness.</param>
    /// <exception cref="BusinessRuleViolationException">Thrown when the participant has not indicated affectedness for this concern.</exception>
    public void WithdrawAffectedness(ParticipantId participantId)
    {
        ArgumentNullException.ThrowIfNull(participantId);

        if (!_affectedParticipants.Contains(participantId))
        {
            throw new BusinessRuleViolationException("Participant has not indicated affectedness for this concern.");
        }

        ApplyChange(new AffectednessWithdrawn(Id.Value, participantId.Value));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case ConcernRaised e: Apply(e); break;
            case AffectednessIndicated e: Apply(e); break;
            case AffectednessWithdrawn e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(ConcernRaised e)
    {
        Id = ConcernId.Create(e.AggregateId);
    }

    private void Apply(AffectednessIndicated e)
    {
        var participantId = ParticipantId.Create(e.IndicatedById);
        _affectedParticipants.Add(participantId);
    }

    private void Apply(AffectednessWithdrawn e)
    {
        var participantId = ParticipantId.Create(e.WithdrawnById);
        _affectedParticipants.Remove(participantId);
    }
}