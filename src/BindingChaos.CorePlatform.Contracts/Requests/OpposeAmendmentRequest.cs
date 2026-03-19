namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for opposing an amendment.
/// </summary>
/// <param name="Reason">The reason provided by the participant for opposing the amendment.</param>
public sealed record OpposeAmendmentRequest(string Reason);
