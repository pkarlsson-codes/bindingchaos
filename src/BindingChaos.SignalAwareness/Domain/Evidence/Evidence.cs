using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SignalAwareness.Domain.Evidence.Events;
using BindingChaos.SignalAwareness.Domain.Signals;

namespace BindingChaos.SignalAwareness.Domain.Evidence;

/// <summary>
/// A piece of evidence about a signal.
/// </summary>
public class Evidence : AggregateRoot<EvidenceId>
{
    /// <summary>
    /// Initializes a new instance of the Evidence class with the specified identifiers, related documents, description,
    /// and participant information.
    /// </summary>
    /// <param name="id">The unique identifier for the evidence instance.</param>
    /// <param name="signalId">The identifier of the signal associated with this evidence.</param>
    /// <param name="documentIds">An array of document identifiers that are related to the evidence. Cannot be null.</param>
    /// <param name="description">A brief description of the evidence.</param>
    /// <param name="addedBy">The identifier of the participant who added the evidence.</param>
    public Evidence(EvidenceId id, SignalId signalId, string[] documentIds, string description, ParticipantId addedBy)
    {
        this.ApplyChange(new EvidenceAdded(id.Value, signalId.Value, documentIds, description, addedBy.Value));
    }

    /// <summary>
    /// Creates a new Evidence instance associated with the specified signal and documents.
    /// </summary>
    /// <param name="signalId">The identifier of the signal to which the evidence is related.</param>
    /// <param name="documentIds">An array of document identifiers that are linked to the evidence. Cannot be null.</param>
    /// <param name="description">A description that provides context or details about the evidence.</param>
    /// <param name="addedBy">The identifier of the participant who is adding the evidence.</param>
    /// <returns>An Evidence object representing the newly created evidence entry.</returns>
    internal static Evidence Add(SignalId signalId, string[] documentIds, string description, ParticipantId addedBy)
    {
        return new Evidence(EvidenceId.Generate(), signalId, documentIds, description, addedBy);
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case EvidenceAdded e: Apply(e); break;
        }
    }

    private void Apply(EvidenceAdded e)
    {
        Id = EvidenceId.Create(e.AggregateId);
    }
}
