using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.ResourceRequirements;
using BindingChaos.Stigmergy.Domain.UserGroups;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to update the quantity needed for a resource requirement.
/// </summary>
/// <param name="RequirementId">The resource requirement to update.</param>
/// <param name="ActorId">The participant making the update.</param>
/// <param name="QuantityNeeded">The new quantity needed.</param>
public sealed record UpdateResourceRequirementQuantity(
    ResourceRequirementId RequirementId,
    ParticipantId ActorId,
    double QuantityNeeded);

/// <summary>
/// Handles the <see cref="UpdateResourceRequirementQuantity"/> command.
/// </summary>
public static class UpdateResourceRequirementQuantityHandler
{
    /// <summary>
    /// Updates the quantity after verifying the actor belongs to the owning user group.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="requirementRepository">Repository used to load and persist the requirement.</param>
    /// <param name="projectRepository">Repository used to load the owning project.</param>
    /// <param name="userGroupRepository">Repository used to verify group membership.</param>
    /// <param name="unitOfWork">Unit of work for transaction boundaries.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        UpdateResourceRequirementQuantity command,
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
            throw new BusinessRuleViolationException("Only user group members can update resource requirements.");
        }

        requirement.UpdateQuantity(command.QuantityNeeded);

        requirementRepository.Stage(requirement);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
