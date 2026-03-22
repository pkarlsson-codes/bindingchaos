using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.Evidence.Events;

/// <summary>
/// Represents an event indicating that evidence has been added to a signal.
/// </summary>
/// <param name="AggregateId">The unique identifier of the evidence.</param>
/// <param name="SignalId">The unique identifier of the added evidence.</param>
/// <param name="DocumentIds">The unique identifiers of any documents associated with the evidence.</param>
/// <param name="Description">A textual description of the evidence.</param>
/// <param name="AddedBy">Id of the participant that added the evidence.</param>
public sealed record EvidenceAdded(
    string AggregateId,
    string SignalId,
    string[] DocumentIds,
    string Description,
    string AddedBy
) : DomainEvent(AggregateId);
