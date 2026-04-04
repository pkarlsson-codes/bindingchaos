using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Concerns.Events;

/// <summary>
/// Indicates that the affectedness of a concern has been indicated by a participant.
/// </summary>
/// <param name="AggregateId">Id of the concern affecting the participant.</param>
/// <param name="IndicatedById">Id of the participant who indicated the affectedness.</param>
public sealed record AffectednessIndicated(
    string AggregateId,
    string IndicatedById
) : DomainEvent(AggregateId);
