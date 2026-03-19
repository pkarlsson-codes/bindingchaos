namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for posting a contribution to a discourse thread.
/// </summary>
/// <param name="Content">The content of the contribution.</param>
/// <param name="ParentContributionId">The ID of the parent contribution if this is a reply (optional).</param>
public record PostContributionRequest(string Content, string? ParentContributionId = null);