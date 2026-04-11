namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>Response model for a project's contestation status.</summary>
/// <param name="OpenInquiryCount">The number of currently open inquiries.</param>
/// <param name="IsContested">True when at least one inquiry is open.</param>
public sealed record ProjectContestationStatusResponse(int OpenInquiryCount, bool IsContested);
