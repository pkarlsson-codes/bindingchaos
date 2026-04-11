namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>Request model for updating or reopening a project inquiry body.</summary>
/// <param name="NewBody">The updated body text.</param>
public sealed record UpdateProjectInquiryRequest(string NewBody);
