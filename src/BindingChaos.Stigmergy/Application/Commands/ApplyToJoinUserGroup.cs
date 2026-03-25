using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;

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
    /// <param name="documentSession">The document session used to retrieve and persist changes to the user group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(ApplyToJoinUserGroup command, IDocumentSession documentSession, CancellationToken cancellationToken)
    {
        var userGroup = await documentSession.LoadAsync<UserGroup>(command.UserGroupId, cancellationToken).ConfigureAwait(false);
        if (userGroup == null)
        {
            throw new InvalidOperationException($"User group with ID {command.UserGroupId.Value} not found.");
        }

        userGroup.ApplyToJoin(command.ParticipantId);
    }
}