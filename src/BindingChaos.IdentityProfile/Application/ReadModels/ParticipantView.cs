namespace BindingChaos.IdentityProfile.Application.ReadModels;

/// <summary>
/// A public-facing view of a participant's identity, resolved by pseudonym.
/// </summary>
/// <param name="UserId">The participant's internal stable identifier.</param>
/// <param name="Pseudonym">The participant's globally unique pseudonym.</param>
/// <param name="JoinedAt">The UTC timestamp when the participant registered.</param>
public sealed record ParticipantView(string UserId, string Pseudonym, DateTimeOffset JoinedAt);
