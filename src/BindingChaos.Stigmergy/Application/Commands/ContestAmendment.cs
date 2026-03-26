using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Wolverine;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to contest an Active amendment on a project. The contester must be a member
/// of the user group that owns the project.
/// </summary>
/// <param name="ProjectId">The identifier of the project containing the amendment.</param>
/// <param name="AmendmentId">The identifier of the amendment to contest.</param>
/// <param name="ContesterId">The identifier of the participant contesting the amendment.</param>
public sealed record ContestAmendment(ProjectId ProjectId, AmendmentId AmendmentId, ParticipantId ContesterId);

/// <summary>
/// Handles the <see cref="ContestAmendment"/> command.
/// </summary>
public static class ContestAmendmentHandler
{
    /// <summary>
    /// Contests an Active amendment, transitions it to Contested, and starts the contention saga.
    /// </summary>
    /// <param name="command">The command containing contention details.</param>
    /// <param name="projectRepository">The repository to retrieve and persist the project.</param>
    /// <param name="userGroupRepository">The repository to retrieve the user group.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="messageBus">The Wolverine message bus.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ContestAmendment command,
        IProjectRepository projectRepository,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var project = await projectRepository.GetByIdOrThrowAsync(command.ProjectId, cancellationToken).ConfigureAwait(false);
        var userGroup = await userGroupRepository.GetByIdOrThrowAsync(project.UserGroupId, cancellationToken).ConfigureAwait(false);

        if (!userGroup.Members.Any(m => m.ParticipantId == command.ContesterId))
        {
            throw new BusinessRuleViolationException("Only user group members can contest amendments.");
        }

        project.ContestAmendment(command.AmendmentId, command.ContesterId);
        projectRepository.Stage(project);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        await messageBus.PublishAsync(new AmendmentContentionStarted(
            command.AmendmentId.Value,
            command.ProjectId.Value,
            project.UserGroupId.Value,
            userGroup.Charter.ContentionRules.RejectionThreshold,
            userGroup.Charter.ContentionRules.ResolutionWindow,
            command.ContesterId.Value)).ConfigureAwait(false);
    }
}
