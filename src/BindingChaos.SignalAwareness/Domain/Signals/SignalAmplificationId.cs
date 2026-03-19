using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Unique identifier for a <see cref="SignalAmplification"/>.
/// </summary>
internal sealed class SignalAmplificationId : EntityId<SignalAmplificationId>
{
    private const string Prefix = "amplification";

    private SignalAmplificationId(string value)
        : base(value, Prefix)
    {
    }
}