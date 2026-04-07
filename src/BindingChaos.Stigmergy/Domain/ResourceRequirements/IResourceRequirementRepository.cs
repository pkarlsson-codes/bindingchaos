using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Stigmergy.Domain.ResourceRequirements;

/// <summary>
/// Repository interface for <see cref="ResourceRequirement"/> aggregates.
/// </summary>
public interface IResourceRequirementRepository : IRepository<ResourceRequirement, ResourceRequirementId>
{
}
