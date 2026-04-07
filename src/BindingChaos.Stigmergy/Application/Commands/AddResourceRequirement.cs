using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.ResourceRequirements;
using BindingChaos.Stigmergy.Domain.UserGroups;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to add a resource requirement to a project.
/// </summary>
/// <param name="ProjectId">The project to add the requirement to.</param>
/// <param name="ActorId">The participant adding the requirement.</param>
/// <param name="Description">A human-readable description of what is needed.</param>
/// <param name="QuantityNeeded">The total quantity of the resource required.</param>
/// <param name="Unit">The unit of measure (e.g. "hours", "kg").</param>
public sealed record AddResourceRequirement(
    ProjectId ProjectId,
    ParticipantId ActorId,
    string Description,
    double QuantityNeeded,
    string Unit);

/// <summary>
/// Handles the <see cref="AddResourceRequirement"/> command.
/// </summary>
public static class AddResourceRequirementHandler
{
    /// <summary>
    /// Adds a resource requirement after verifying the actor belongs to the owning user group.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="projectRepository">Repository used to load the project.</param>
    /// <param name="userGroupRepository">Repository used to verify group membership.</param>
    /// <param name="requirementRepository">Repository used to persist the requirement.</param>
    /// <param name="unitOfWork">Unit of work for transaction boundaries.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identifier of the created resource requirement.</returns>
    public static async Task<ResourceRequirementId> Handle(
        AddResourceRequirement command,
        IProjectRepository projectRepository,
        IUserGroupRepository userGroupRepository,
        IResourceRequirementRepository requirementRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var project = await projectRepository
            .GetByIdOrThrowAsync(command.ProjectId, cancellationToken)
            .ConfigureAwait(false);

        var userGroup = await userGroupRepository
            .GetByIdOrThrowAsync(project.UserGroupId, cancellationToken)
            .ConfigureAwait(false);

        if (!userGroup.Members.Any(m => m.ParticipantId == command.ActorId))
        {
            throw new BusinessRuleViolationException("Only user group members can add resource requirements.");
        }

        var requirement = ResourceRequirement.Create(
            command.ProjectId,
            command.Description,
            command.QuantityNeeded,
            command.Unit);

        requirementRepository.Stage(requirement);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return requirement.Id;
    }
}
