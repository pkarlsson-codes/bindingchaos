using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Projects.Events;

/// <summary>
/// Domain event raised when a new project is created.
/// </summary>
/// <param name="ProjectId">The identifier of the newly created project.</param>
/// <param name="UserGroupId">The identifier of the user group that owns the project.</param>
/// <param name="Title">The title of the project.</param>
/// <param name="Description">The description of the project.</param>
internal sealed record ProjectCreated(
    string ProjectId,
    string UserGroupId,
    string Title,
    string Description)
    : DomainEvent(ProjectId, 0);
