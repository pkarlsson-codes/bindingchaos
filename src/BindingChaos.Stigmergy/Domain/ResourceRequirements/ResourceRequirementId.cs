using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.ResourceRequirements;

/// <summary>
/// Unique identifier for a <see cref="ResourceRequirement"/>.
/// </summary>
public sealed class ResourceRequirementId : EntityId<ResourceRequirementId>
{
    private const string Prefix = "requirement";

    private ResourceRequirementId(string value)
        : base(value, Prefix)
    {
    }
}
