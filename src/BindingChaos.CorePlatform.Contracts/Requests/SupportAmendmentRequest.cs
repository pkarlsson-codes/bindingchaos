namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for supporting an amendment.
/// </summary>
/// <param name="Reason">The reason provided by the participant for supporting the amendment.</param>
public sealed record SupportAmendmentRequest(string Reason);
