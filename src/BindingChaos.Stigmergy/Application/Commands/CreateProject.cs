using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to create a new project under a user group.
/// </summary>
/// <param name="UserGroupId">The user group that will own the project.</param>
/// <param name="ActorId">The participant creating the project.</param>
/// <param name="Title">The project title.</param>
/// <param name="Description">The project description.</param>
public sealed record CreateProject(UserGroupId UserGroupId, ParticipantId ActorId, string Title, string Description);

/// <summary>
/// Handles the <see cref="CreateProject"/> command.
/// </summary>
public static class CreateProjectHandler
{
    /// <summary>
    /// Creates a project after verifying the actor belongs to the owning user group.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="projectRepository">Repository used to persist the project.</param>
    /// <param name="userGroupRepository">Repository used to load the owning user group.</param>
    /// <param name="unitOfWork">Unit of work for transaction boundaries.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identifier of the created project.</returns>
    public static async Task<ProjectId> Handle(
        CreateProject command,
        IProjectRepository projectRepository,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var userGroup = await userGroupRepository.GetByIdOrThrowAsync(command.UserGroupId, cancellationToken).ConfigureAwait(false);
        if (!userGroup.Members.Any(m => m.ParticipantId == command.ActorId))
        {
            throw new BusinessRuleViolationException("Only user group members can create projects.");
        }

        var project = Project.Create(command.UserGroupId, command.Title, command.Description);
        projectRepository.Stage(project);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return project.Id;
    }
}
