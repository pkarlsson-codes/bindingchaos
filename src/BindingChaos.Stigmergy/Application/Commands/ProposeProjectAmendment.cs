using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to propose a new amendment against a project.
/// </summary>
/// <param name="ProjectId">The project to amend.</param>
/// <param name="ProposerId">The participant proposing the amendment.</param>
public sealed record ProposeProjectAmendment(ProjectId ProjectId, ParticipantId ProposerId);

/// <summary>
/// Handles the <see cref="ProposeProjectAmendment"/> command.
/// </summary>
public static class ProposeProjectAmendmentHandler
{
    /// <summary>
    /// Proposes an amendment for a project after verifying proposer membership.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="projectRepository">Repository used to load and persist the project.</param>
    /// <param name="userGroupRepository">Repository used to verify user-group membership.</param>
    /// <param name="unitOfWork">Unit of work for transaction boundaries.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The identifier of the proposed amendment.</returns>
    public static async Task<AmendmentId> Handle(
        ProposeProjectAmendment command,
        IProjectRepository projectRepository,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var project = await projectRepository.GetByIdOrThrowAsync(command.ProjectId, cancellationToken).ConfigureAwait(false);
        var userGroup = await userGroupRepository.GetByIdOrThrowAsync(project.UserGroupId, cancellationToken).ConfigureAwait(false);

        if (!userGroup.Members.Any(m => m.ParticipantId == command.ProposerId))
        {
            throw new BusinessRuleViolationException("Only user group members can propose amendments.");
        }

        var amendmentId = project.ProposeAmendment(command.ProposerId);
        projectRepository.Stage(project);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        return amendmentId;
    }
}
