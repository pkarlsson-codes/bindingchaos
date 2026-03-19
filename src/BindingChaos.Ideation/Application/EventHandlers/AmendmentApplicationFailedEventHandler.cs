using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Amendments.Events;
using BindingChaos.SharedKernel.Persistence;
using Microsoft.Extensions.Logging;

namespace BindingChaos.Ideation.Application.EventHandlers;

/// <summary>
/// Event handler that marks approved amendments as outdated when
/// they fail to apply to the target idea due to version mismatches.
/// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public static partial class AmendmentApplicationFailedEventHandler
#pragma warning restore CA1711
{
    /// <summary>
    /// Handles the AmendmentApplicationFailed event by marking the amendment as outdated
    /// if the failure was due to a version mismatch.
    /// </summary>
    /// <param name="notification">The amendment application failed event.</param>
    /// <param name="amendmentRepository">Repository for loading and saving amendments.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="logger">Logger for diagnostic output.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        AmendmentApplicationFailed notification,
        IAmendmentRepository amendmentRepository,
        IUnitOfWork unitOfWork,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (notification.Reason != "VersionMismatch")
        {
            Logs.NonVersionMismatchFailure(logger, notification.AggregateId, notification.Reason);
            return;
        }

        if (!notification.IdeaCurrentVersion.HasValue)
        {
            Logs.MissingCurrentVersion(logger, notification.AggregateId);
            return;
        }

        var amendmentId = AmendmentId.Create(notification.AggregateId);
        var amendment = await amendmentRepository
            .GetByIdAsync(amendmentId, cancellationToken)
            .ConfigureAwait(false);

        if (amendment == null)
        {
            Logs.AmendmentNotFound(logger, amendmentId.Value);
            return;
        }

        Logs.MarkingAmendmentOutdated(logger, amendment.Id.Value, amendment.TargetVersionNumber, notification.IdeaCurrentVersion.Value);

        amendment.MarkOutdatedAfterApplicationFailure(notification.IdeaCurrentVersion.Value);
        amendmentRepository.Stage(amendment);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        Logs.AmendmentMarkedOutdated(logger, amendment.Id.Value);
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Amendment {AmendmentId} application failed with reason '{Reason}'. No compensating action needed.")]
        internal static partial void NonVersionMismatchFailure(ILogger logger, string amendmentId, string reason);

        [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "Amendment {AmendmentId} application failed due to VersionMismatch but no current version provided")]
        internal static partial void MissingCurrentVersion(ILogger logger, string amendmentId);

        [LoggerMessage(EventId = 3, Level = LogLevel.Warning, Message = "Cannot handle application failure for amendment {AmendmentId}: Amendment not found")]
        internal static partial void AmendmentNotFound(ILogger logger, string amendmentId);

        [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Marking approved amendment {AmendmentId} as outdated after application failure. Target version: {TargetVersion}, Actual version: {ActualVersion}")]
        internal static partial void MarkingAmendmentOutdated(ILogger logger, string amendmentId, int targetVersion, int actualVersion);

        [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Amendment {AmendmentId} successfully marked as outdated")]
        internal static partial void AmendmentMarkedOutdated(ILogger logger, string amendmentId);
    }
}
