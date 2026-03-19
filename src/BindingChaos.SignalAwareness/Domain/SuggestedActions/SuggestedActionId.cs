using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.SuggestedActions;

/// <summary>
/// Unique identifier for a <see cref="SuggestedAction"/>.
/// </summary>
public sealed class SuggestedActionId : EntityId<SuggestedActionId>
{
    private const string Prefix = "signalaction";

    private SuggestedActionId(string value)
        : base(value, Prefix)
    {
    }
}