namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model for amendment voting operations (support, oppose, withdraw support, withdraw opposition).
/// </summary>
/// <param name="SupporterCount">The updated number of supporters for this amendment.</param>
/// <param name="OpponentCount">The updated number of opponents for this amendment.</param>
public sealed record AmendmentVoteResponse(
    int SupporterCount,
    int OpponentCount);
