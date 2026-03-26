using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command for a participant to leave a user group.
/// </summary>
/// <param name="UserGroupId">The ID of the user group to leave.</param>
/// <param name="ParticipantId">The ID of the participant leaving the group.</param>
public sealed record LeaveUserGroup(UserGroupId UserGroupId, ParticipantId ParticipantId);

/// <summary>
/// Handler for the <see cref="LeaveUserGroup"/> command.
/// </summary>
public static class LeaveUserGroupHandler
{
    /// <summary>
    /// Handles a participant leaving a user group.
    /// </summary>
    /// <param name="command">The command containing the group and participant identifiers.</param>
    /// <param name="userGroupRepository">The repository to retrieve and persist the user group.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        LeaveUserGroup command,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var userGroup = await userGroupRepository.GetByIdAsync(command.UserGroupId, cancellationToken).ConfigureAwait(false);
        if (userGroup is null)
        {
            throw new AggregateNotFoundException(typeof(UserGroup), command.UserGroupId);
        }

        userGroup.Leave(command.ParticipantId);

        userGroupRepository.Stage(userGroup);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
