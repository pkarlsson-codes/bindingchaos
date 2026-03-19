namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for amendment proposal.
/// </summary>
/// <param name="AmendmentId">The ID of the created amendment.</param>
public record ProposeAmendmentResponse(string AmendmentId);