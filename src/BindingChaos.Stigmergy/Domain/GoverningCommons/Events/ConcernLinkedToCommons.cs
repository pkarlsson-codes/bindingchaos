using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons.Events;

/// <summary>Domain event raised when a Commons links itself to a Concern it is organising around.</summary>
/// <param name="AggregateId">The unique identifier of the commons.</param>
/// <param name="ConcernId">The identifier of the concern being linked.</param>
/// <param name="ClaimedByParticipantId">The identifier of the participant who claimed the concern on behalf of this commons.</param>
public sealed record ConcernLinkedToCommons(
    string AggregateId,
    string ConcernId,
    string ClaimedByParticipantId)
    : DomainEvent(AggregateId);
