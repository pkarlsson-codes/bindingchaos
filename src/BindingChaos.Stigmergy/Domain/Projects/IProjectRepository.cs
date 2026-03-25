using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Stigmergy.Domain.Projects;

/// <summary>
/// Repository interface for <see cref="Project"/> aggregates.
/// </summary>
public interface IProjectRepository : IRepository<Project, ProjectId>
{
}