using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Projects;
using Marten;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Stigmergy.Infrastructure.Persistence;

/// <summary>
/// Marten implementation of the Project repository for event sourcing.
/// </summary>
/// <param name="session">The Marten document session.</param>
/// <param name="logger">The logger for the repository.</param>
internal class ProjectRepository(
    IDocumentSession session,
    ILogger<MartenRepository<Project, ProjectId>> logger)
    : MartenRepository<Project, ProjectId>(session, logger), IProjectRepository
{
}