namespace BindingChaos.IdentityProfile.Application.ReadModels;

/// <summary>
/// A read-only view of an invite link.
/// </summary>
/// <param name="Id">The invite link ID.</param>
/// <param name="Token">The URL-safe base64url token.</param>
/// <param name="Note">The optional private note visible only to the creator.</param>
/// <param name="IsRevoked">Whether the link has been revoked.</param>
/// <param name="CreatedAt">UTC timestamp of creation.</param>
public sealed record InviteLinkView(Guid Id, string Token, string? Note, bool IsRevoked, DateTimeOffset CreatedAt);
