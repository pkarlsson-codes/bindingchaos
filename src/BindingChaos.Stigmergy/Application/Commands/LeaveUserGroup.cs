using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command for a participant to leave a user group.
/// </summary>
/// <param name="UserGroupId">The ID of the user group to leave.</param>
/// <param name="ParticipantId">The ID of the participant leaving the group.</param>
public sealed record LeaveUserGroup(string UserGroupId, string ParticipantId);

/// <summary>
/// Handler for the <see cref="LeaveUserGroup"/> command.
/// </summary>
public static class LeaveUserGroupHandler
{
    /// <summary>
    /// Handles a participant leaving a user group.
    /// </summary>
    /// <param name="command">The command containing the group and participant identifiers.</param>
    /// <param name="documentSession">The document session used to retrieve and persist changes to the user group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(LeaveUserGroup command, IDocumentSession documentSession, CancellationToken cancellationToken)
    {
        var userGroup = await documentSession.LoadAsync<UserGroup>(UserGroupId.Create(command.UserGroupId), cancellationToken).ConfigureAwait(false);
        if (userGroup is null)
        {
            throw new AggregateNotFoundException(typeof(UserGroup), command.UserGroupId);
        }

        userGroup.Leave(ParticipantId.Create(command.ParticipantId));

        documentSession.Store(userGroup);
        await documentSession.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
