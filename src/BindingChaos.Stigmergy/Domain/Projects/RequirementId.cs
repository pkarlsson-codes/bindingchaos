using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// Unique identifier for a <see cref="Requirement"/>.
/// </summary>
public sealed class RequirementId : EntityId<RequirementId>
{
    private const string Prefix = "requirement";

    private RequirementId(string value)
        : base(value, Prefix)
    {
    }
}
