namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for creating a society invite link.
/// </summary>
/// <param name="Note">An optional private note visible only to the creator.</param>
public record CreateSocietyInviteLinkRequest(string? Note);