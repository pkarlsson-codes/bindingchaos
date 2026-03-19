namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for joining a society.
/// </summary>
/// <param name="SocialContractId">The ID of the social contract the participant is agreeing to.</param>
public record JoinSocietyRequest(string SocialContractId);
