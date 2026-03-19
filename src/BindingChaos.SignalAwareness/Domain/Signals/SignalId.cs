using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Unique identifier for a <see cref="Signal"/>.
/// </summary>
public sealed class SignalId : EntityId<SignalId>
{
    private const string Prefix = "signal";

    private SignalId(string value)
        : base(value, Prefix)
    {
    }
}