namespace BindingChaos.IdentityProfile.Application.ReadModels;

/// <summary>
/// A read-only view of a society invite link.
/// </summary>
/// <param name="Id">The invite link ID.</param>
/// <param name="Token">The URL-safe base64url token.</param>
/// <param name="SocietyId">The ID of the society this invite link is for.</param>
/// <param name="Note">An optional private note visible only to the creator.</param>
/// <param name="IsRevoked">Whether the link has been revoked.</param>
/// <param name="CreatedAt">UTC timestamp of creation.</param>
public sealed record SocietyInviteLinkView(Guid Id, string Token, string SocietyId, string? Note, bool IsRevoked, DateTimeOffset CreatedAt);
