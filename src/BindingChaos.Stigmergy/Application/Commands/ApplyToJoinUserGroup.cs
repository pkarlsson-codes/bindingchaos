using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.UserGroups;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to apply for membership in a user group, containing the necessary identifiers for the user group and participant involved in the application process.
/// </summary>
/// <param name="UserGroupId">The ID of the user group to which the participant is applying.</param>
/// <param name="ParticipantId">The ID of the participant applying for membership.</param>
public sealed record ApplyToJoinUserGroup(UserGroupId UserGroupId, ParticipantId ParticipantId);

/// <summary>
/// Handler for the <see cref="ApplyToJoinUserGroup"/> command.
/// </summary>
public static class ApplyToJoinUserGroupHandler
{
    /// <summary>
    /// Handles the application for membership in a user group by processing the provided command.
    /// </summary>
    /// <param name="command">The command containing the details of the application for membership.</param>
    /// <param name="userGroupRepository">The repository to retrieve and persist the user group.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ApplyToJoinUserGroup command,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var userGroup = await userGroupRepository.GetByIdOrThrowAsync(command.UserGroupId, cancellationToken).ConfigureAwait(false);

        userGroup.ApplyToJoin(command.ParticipantId);

        userGroupRepository.Stage(userGroup);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
