using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.ResourceRequirements;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the ResourceRequirement repository for event sourcing.
/// </summary>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger for the repository.</param>
internal sealed class ResourceRequirementRepository(
    IDocumentSession session,
    ILogger<MartenRepository<ResourceRequirement, ResourceRequirementId>> logger)
    : MartenRepository<ResourceRequirement, ResourceRequirementId>(session, logger), IResourceRequirementRepository
{
}
