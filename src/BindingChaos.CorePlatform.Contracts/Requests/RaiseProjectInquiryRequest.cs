namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>Request model for raising a project inquiry.</summary>
/// <param name="Body">The inquiry body text.</param>
public sealed record RaiseProjectInquiryRequest(string Body);
