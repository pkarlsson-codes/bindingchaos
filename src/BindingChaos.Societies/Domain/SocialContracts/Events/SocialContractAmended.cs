using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.SocialContracts.Events;

/// <summary>
/// Domain event raised when a social contract is amended.
/// </summary>
/// <param name="AggregateId">The ID of the social contract.</param>
/// <param name="AmendedBy">The participant ID who made the amendment.</param>
/// <param name="RatificationThreshold">The new ratification threshold, or null if unchanged.</param>
/// <param name="ReviewWindowTicks">The new review window in ticks, or null if unchanged.</param>
/// <param name="AllowVeto">The new veto flag, or null if unchanged.</param>
/// <param name="RequiredVerificationWeight">The new required verification weight, or null if unchanged.</param>
public sealed record SocialContractAmended(
    string AggregateId,
    string AmendedBy,
    double? RatificationThreshold,
    long? ReviewWindowTicks,
    bool? AllowVeto,
    double? RequiredVerificationWeight
) : DomainEvent(AggregateId);
