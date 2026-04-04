using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Concerns.Events;

/// <summary>
/// Indicates that the affectedness of a concern has been withdrawn by a participant.
/// </summary>
/// <param name="AggregateId">Id of the concern affecting the participant.</param>
/// <param name="WithdrawnById">Id of the participant who withdrew the affectedness.</param>
public sealed record AffectednessWithdrawn(
    string AggregateId,
    string WithdrawnById
) : DomainEvent(AggregateId);