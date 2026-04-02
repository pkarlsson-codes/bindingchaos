using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Domain event raised when a participant joins a society.
/// </summary>
/// <param name="AggregateId">The ID of the society.</param>
/// <param name="MembershipId">The new membership identifier.</param>
/// <param name="ParticipantId">The ID of the participant who joined.</param>
/// <param name="SocialContractId">The ID of the social contract agreed to.</param>
/// <param name="InviteToken">The invite token used to join, if any. Used for attribution only.</param>
public sealed record MemberJoined(
    string AggregateId,
    string MembershipId,
    string ParticipantId,
    string SocialContractId,
    string? InviteToken = null
) : DomainEvent(AggregateId);
