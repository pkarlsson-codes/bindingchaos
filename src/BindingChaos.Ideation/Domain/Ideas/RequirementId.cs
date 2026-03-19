using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// Unique identifier for a <see cref="Requirement"/>.
/// </summary>
public class RequirementId : EntityId<RequirementId>
{
    private const string Prefix = "requirement";

    private RequirementId(string value)
        : base(value, Prefix)
    {
    }
}