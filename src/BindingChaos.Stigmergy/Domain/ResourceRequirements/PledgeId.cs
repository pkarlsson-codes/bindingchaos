using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.ResourceRequirements;

/// <summary>
/// Unique identifier for a <see cref="Pledge"/>.
/// </summary>
public sealed class PledgeId : EntityId<PledgeId>
{
    private const string Prefix = "pledge";

    private PledgeId(string value)
        : base(value, Prefix)
    {
    }
}
