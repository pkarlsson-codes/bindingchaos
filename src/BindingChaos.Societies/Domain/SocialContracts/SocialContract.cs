using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Societies.Domain.SocialContracts.Events;

namespace BindingChaos.Societies.Domain.SocialContracts;

/// <summary>
/// Aggregate root representing a social contract — the rules of governance for a society.
/// </summary>
public sealed class SocialContract : AggregateRoot<SocialContractId>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    private SocialContract()
    {
    }
#pragma warning restore CS8618

    /// <summary>
    /// Gets the decision protocol for this social contract.
    /// </summary>
    public DecisionProtocol Protocol { get; private set; }

    /// <summary>
    /// Gets the epistemic rules for this social contract.
    /// </summary>
    public EpistemicRules EpistemicRules { get; private set; }

    /// <summary>
    /// Establishes a new social contract for a society.
    /// </summary>
    /// <param name="societyId">The society this contract governs.</param>
    /// <param name="establishedBy">The participant establishing the contract.</param>
    /// <param name="protocol">The decision protocol.</param>
    /// <param name="epistemicRules">The epistemic rules.</param>
    /// <returns>A new <see cref="SocialContract"/> instance.</returns>
    public static SocialContract Establish(
        SocietyId societyId,
        ParticipantId establishedBy,
        DecisionProtocol protocol,
        EpistemicRules epistemicRules)
    {
        ArgumentNullException.ThrowIfNull(societyId);
        ArgumentNullException.ThrowIfNull(establishedBy);
        ArgumentNullException.ThrowIfNull(protocol);
        ArgumentNullException.ThrowIfNull(epistemicRules);

        var contract = new SocialContract();
        var id = SocialContractId.Generate();

        contract.ApplyChange(new SocialContractEstablished(
            id.Value,
            contract.Version,
            societyId.Value,
            establishedBy.Value,
            protocol.RatificationThreshold,
            protocol.ReviewWindow.Ticks,
            protocol.AllowVeto,
            epistemicRules.RequiredVerificationWeight));

        return contract;
    }

    /// <summary>
    /// Amends the social contract. At least one of <paramref name="newProtocol"/> or <paramref name="newEpistemicRules"/> must be non-null.
    /// </summary>
    /// <param name="amendedBy">The participant making the amendment.</param>
    /// <param name="newProtocol">The new decision protocol, or null to keep the current one.</param>
    /// <param name="newEpistemicRules">The new epistemic rules, or null to keep the current ones.</param>
    public void Amend(ParticipantId amendedBy, DecisionProtocol? newProtocol, EpistemicRules? newEpistemicRules)
    {
        ArgumentNullException.ThrowIfNull(amendedBy);

        if (newProtocol is null && newEpistemicRules is null)
        {
            throw new BusinessRuleViolationException(
                "At least one of newProtocol or newEpistemicRules must be provided when amending a social contract.");
        }

        ApplyChange(new SocialContractAmended(
            Id.Value,
            Version,
            amendedBy.Value,
            newProtocol?.RatificationThreshold,
            newProtocol?.ReviewWindow.Ticks,
            newProtocol?.AllowVeto,
            newEpistemicRules?.RequiredVerificationWeight));
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SocialContractEstablished e: Apply(e); break;
            case SocialContractAmended e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent?.GetType().Name}");
        }
    }

    private void Apply(SocialContractEstablished evt)
    {
        Id = SocialContractId.Create(evt.AggregateId);
        Protocol = new DecisionProtocol(evt.RatificationThreshold, TimeSpan.FromTicks(evt.ReviewWindowTicks), evt.AllowVeto);
        EpistemicRules = new EpistemicRules(evt.RequiredVerificationWeight);
    }

    private void Apply(SocialContractAmended evt)
    {
        if (evt.RatificationThreshold.HasValue || evt.ReviewWindowTicks.HasValue || evt.AllowVeto.HasValue)
        {
            Protocol = new DecisionProtocol(
                evt.RatificationThreshold ?? Protocol.RatificationThreshold,
                evt.ReviewWindowTicks.HasValue ? TimeSpan.FromTicks(evt.ReviewWindowTicks.Value) : Protocol.ReviewWindow,
                evt.AllowVeto ?? Protocol.AllowVeto);
        }

        if (evt.RequiredVerificationWeight.HasValue)
        {
            EpistemicRules = new EpistemicRules(evt.RequiredVerificationWeight.Value);
        }
    }
}
