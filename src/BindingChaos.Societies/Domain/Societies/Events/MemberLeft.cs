using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Domain event raised when a participant leaves a society.
/// </summary>
/// <param name="AggregateId">The ID of the society.</param>
/// <param name="MembershipId">The membership identifier of the leaving participant.</param>
/// <param name="ParticipantId">The ID of the participant who left.</param>
public sealed record MemberLeft(
    string AggregateId,
    string MembershipId,
    string ParticipantId
) : DomainEvent(AggregateId);
