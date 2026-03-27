namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response returned when an invite link is successfully created.
/// </summary>
/// <param name="Id">The invite link ID.</param>
/// <param name="Token">The URL-safe base64url token.</param>
/// <param name="Note">The optional private note supplied by the creator.</param>
/// <param name="CreatedAt">UTC timestamp of creation.</param>
public sealed record TrustInviteLinkCreatedResponse(Guid Id, string Token, string? Note, DateTimeOffset CreatedAt);
