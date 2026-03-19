using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.SocialContracts.Events;

/// <summary>
/// Domain event raised when a social contract is established for a society.
/// </summary>
/// <param name="AggregateId">The ID of the social contract.</param>
/// <param name="Version">The aggregate version when this event was raised.</param>
/// <param name="SocietyId">The ID of the society this contract belongs to.</param>
/// <param name="EstablishedBy">The participant ID who established this contract.</param>
/// <param name="RatificationThreshold">The ratification threshold from the decision protocol.</param>
/// <param name="ReviewWindowTicks">The review window duration in ticks.</param>
/// <param name="AllowVeto">Whether veto is allowed.</param>
/// <param name="RequiredVerificationWeight">The required verification weight from the epistemic rules.</param>
public sealed record SocialContractEstablished(
    string AggregateId,
    long Version,
    string SocietyId,
    string EstablishedBy,
    double RatificationThreshold,
    long ReviewWindowTicks,
    bool AllowVeto,
    double RequiredVerificationWeight
) : DomainEvent(AggregateId, Version);
