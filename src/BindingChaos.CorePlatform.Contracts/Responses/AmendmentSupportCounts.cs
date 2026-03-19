namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Represents the support and opposition counts for an amendment.
/// </summary>
/// <param name="SupporterCount">The number of participants supporting the amendment.</param>
/// <param name="OpponentCount">The number of participants opposing the amendment.</param>
public sealed record AmendmentSupportCounts(
    int SupporterCount,
    int OpponentCount);