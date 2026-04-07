using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.ResourceRequirements;
using BindingChaos.Stigmergy.Domain.UserGroups;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to pledge resources toward a resource requirement.
/// </summary>
/// <param name="RequirementId">The resource requirement to pledge toward.</param>
/// <param name="ActorId">The participant making the pledge.</param>
/// <param name="Amount">The amount of resources being pledged.</param>
public sealed record PledgeResources(
    ResourceRequirementId RequirementId,
    ParticipantId ActorId,
    double Amount);

/// <summary>
/// Handles the <see cref="PledgeResources"/> command.
/// </summary>
public static class PledgeResourcesHandler
{
    /// <summary>
    /// Pledges resources after verifying the actor belongs to the owning user group.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="requirementRepository">Repository used to load and persist the requirement.</param>
    /// <param name="projectRepository">Repository used to load the owning project.</param>
    /// <param name="userGroupRepository">Repository used to verify group membership.</param>
    /// <param name="unitOfWork">Unit of work for transaction boundaries.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identifier of the created pledge.</returns>
    public static async Task<PledgeId> Handle(
        PledgeResources command,
        IResourceRequirementRepository requirementRepository,
        IProjectRepository projectRepository,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var requirement = await requirementRepository
            .GetByIdOrThrowAsync(command.RequirementId, cancellationToken)
            .ConfigureAwait(false);

        var project = await projectRepository
            .GetByIdOrThrowAsync(requirement.ProjectId, cancellationToken)
            .ConfigureAwait(false);

        var userGroup = await userGroupRepository
            .GetByIdOrThrowAsync(project.UserGroupId, cancellationToken)
            .ConfigureAwait(false);

        if (!userGroup.Members.Any(m => m.ParticipantId == command.ActorId))
        {
            throw new BusinessRuleViolationException("Only user group members can pledge resources.");
        }

        var pledgeId = requirement.PledgeResources(command.ActorId, command.Amount);

        requirementRepository.Stage(requirement);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return pledgeId;
    }
}
