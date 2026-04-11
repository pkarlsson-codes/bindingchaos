namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>Request model for responding to a project inquiry.</summary>
/// <param name="Response">The response text from the user group.</param>
public sealed record RespondToProjectInquiryRequest(string Response);
