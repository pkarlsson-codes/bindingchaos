namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for joining a society.
/// </summary>
/// <param name="SocialContractId">The ID of the social contract the participant is agreeing to.</param>
/// <param name="InviteToken">The invite token from the invite link URL, if the participant arrived via an invite link. Stored for attribution.</param>
public record JoinSocietyRequest(string SocialContractId, string? InviteToken = null);
