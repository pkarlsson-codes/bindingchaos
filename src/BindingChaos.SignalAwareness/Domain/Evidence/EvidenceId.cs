using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.Evidence;

/// <summary>
/// Unique identifier for an <see cref="Evidence"/>.
/// </summary>
public class EvidenceId : EntityId<EvidenceId>
{
    private const string Prefix = "evidence";

    private EvidenceId(string value)
        : base(value, Prefix)
    {
    }
}