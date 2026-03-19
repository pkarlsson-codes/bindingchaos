using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.Ideation.Domain.Ideas;

/// <summary>
/// A requirement for an <see cref="Idea"/>.
/// </summary>
internal class Requirement : Entity<RequirementId>
{
    /// <summary>
    /// Instantiates a new instance of the <see cref="Requirement"/> class.
    /// </summary>
    /// <param name="requirementId">The requirement id.</param>
    /// <param name="spec">The requirement specification.</param>
    public Requirement(RequirementId requirementId, RequirementSpec spec)
    {
        Id = requirementId;
        Specification = spec;
    }

    /// <summary>
    /// Specification of the requirement.
    /// </summary>
    public RequirementSpec Specification { get; }
}
