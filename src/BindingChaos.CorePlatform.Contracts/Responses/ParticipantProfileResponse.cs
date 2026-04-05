namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Public profile information for a participant, resolved by pseudonym.
/// </summary>
/// <param name="UserId">The participant's internal stable identifier.</param>
/// <param name="Pseudonym">The participant's globally unique pseudonym.</param>
/// <param name="JoinedAt">The UTC timestamp when the participant registered.</param>
public sealed record ParticipantProfileResponse(string UserId, string Pseudonym, DateTimeOffset JoinedAt);
