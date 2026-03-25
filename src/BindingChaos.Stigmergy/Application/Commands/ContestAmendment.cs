using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Application.Messages;
using BindingChaos.Stigmergy.Domain.Projects;
using BindingChaos.Stigmergy.Domain.UserGroups;
using Marten;
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
    /// <param name="session">The Marten document session.</param>
    /// <param name="messageBus">The Wolverine message bus.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        ContestAmendment command,
        IDocumentSession session,
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var project = await session.LoadAsync<Project>(command.ProjectId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"Project {command.ProjectId.Value} not found.");

        var userGroup = await session.LoadAsync<UserGroup>(project.UserGroupId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"User group {project.UserGroupId.Value} not found.");

        if (!userGroup.Members.Any(m => m.ParticipantId == command.ContesterId))
        {
            throw new BusinessRuleViolationException("Only user group members can contest amendments.");
        }

        project.ContestAmendment(command.AmendmentId, command.ContesterId);
        session.Store(project);
        await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await messageBus.PublishAsync(new AmendmentContentionStarted(
            command.AmendmentId.Value,
            command.ProjectId.Value,
            project.UserGroupId.Value,
            userGroup.Charter.ContentionRules.RejectionThreshold,
            userGroup.Charter.ContentionRules.ResolutionWindow,
            command.ContesterId.Value)).ConfigureAwait(false);
    }
}
