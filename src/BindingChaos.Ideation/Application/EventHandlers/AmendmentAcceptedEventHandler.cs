using BindingChaos.Ideation.Application.Commands;
using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Amendments.Events;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Ideation.Application.EventHandlers;

/// <summary>
/// Event handler that dispatches the AmendIdea command when an amendment is accepted.
/// This completes the amendment approval workflow by applying the accepted
/// amendment to the target idea.
/// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public static partial class AmendmentAcceptedEventHandler
#pragma warning restore CA1711
{
    /// <summary>
    /// Handles the AmendmentAccepted event by loading the amendment details and
    /// dispatching an AmendIdea command to apply the changes to the target idea.
    /// </summary>
    /// <param name="notification">The amendment accepted event.</param>
    /// <param name="amendmentRepository">Repository for loading amendment details.</param>
    /// <param name="messageBus">Message bus for dispatching the AmendIdea command.</param>
    /// <param name="logger">Logger for diagnostic output.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        AmendmentAccepted notification,
        IAmendmentRepository amendmentRepository,
        IMessageBus messageBus,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var amendmentId = AmendmentId.Create(notification.AggregateId);
        var amendment = await amendmentRepository
            .GetByIdAsync(amendmentId, cancellationToken)
            .ConfigureAwait(false);

        if (amendment == null)
        {
            Logs.AmendmentNotFound(logger, amendmentId.Value);
            return;
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            Logs.DispatchingAmendIdeaCommand(logger, amendment.Id.Value, amendment.TargetIdeaId.Value, amendment.TargetVersionNumber);
        }

        var command = new AmendIdea(
            amendment.TargetIdeaId,
            amendment.Id,
            amendment.ProposedTitle,
            amendment.ProposedBody,
            amendment.TargetVersionNumber);

        await messageBus.InvokeAsync(command, cancellationToken).ConfigureAwait(false);
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "Cannot apply accepted amendment {AmendmentId}: Amendment not found")]
        internal static partial void AmendmentNotFound(ILogger logger, string amendmentId);

        [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Dispatching AmendIdea command for accepted amendment {AmendmentId} targeting idea {IdeaId} version {TargetVersion}")]
        internal static partial void DispatchingAmendIdeaCommand(ILogger logger, string amendmentId, string ideaId, int targetVersion);
    }
}
