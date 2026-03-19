using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Represents the participant's motivation for amplifying a signal.
/// </summary>
public sealed class AmplificationReason : Enumeration<AmplificationReason>
{
    /// <summary>
    /// Signal is highly relevant to the participant's interests or concerns.
    /// </summary>
    public static readonly AmplificationReason HighRelevance = new(1, nameof(HighRelevance));

    /// <summary>
    /// Participant has direct personal experience with the signal's topic.
    /// </summary>
    public static readonly AmplificationReason PersonalExperience = new(2, nameof(PersonalExperience));

    /// <summary>
    /// Participant has expert knowledge in the signal's domain.
    /// </summary>
    public static readonly AmplificationReason ExpertKnowledge = new(3, nameof(ExpertKnowledge));

    /// <summary>
    /// Signal has significant local impact on the participant's community.
    /// </summary>
    public static readonly AmplificationReason LocalImpact = new(4, nameof(LocalImpact));

    /// <summary>
    /// Signal requires urgent action or attention.
    /// </summary>
    public static readonly AmplificationReason UrgentAction = new(5, nameof(UrgentAction));

    /// <summary>
    /// Signal would benefit the broader community if addressed.
    /// </summary>
    public static readonly AmplificationReason CommunityBenefit = new(6, nameof(CommunityBenefit));

    /// <summary>
    /// Initializes a new instance of the AmplificationReason class.
    /// </summary>
    /// <param name="id">The unique identifier for this reason.</param>
    /// <param name="name">The name of this reason.</param>
    private AmplificationReason(int id, string name)
        : base(id, name)
    {
    }
}