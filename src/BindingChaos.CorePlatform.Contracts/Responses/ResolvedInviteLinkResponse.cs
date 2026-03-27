namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// The result of resolving an invite link token.
/// </summary>
/// <param name="InviterUserId">The internal user ID of the participant who created the invite link.</param>
public sealed record ResolvedInviteLinkResponse(string InviterUserId);
