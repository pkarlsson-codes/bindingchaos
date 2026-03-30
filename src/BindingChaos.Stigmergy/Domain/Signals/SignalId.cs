using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Signals;

/// <summary>
/// The uniqe identifier of a <see cref="Signal"/>.
/// </summary>
public class SignalId : EntityId<SignalId>
{
    private const string Prefix = "signal";

    private SignalId(string value)
        : base(value, Prefix)
    {
    }
}