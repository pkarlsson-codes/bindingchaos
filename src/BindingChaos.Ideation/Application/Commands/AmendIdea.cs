using BindingChaos.Ideation.Domain.Amendments;
using BindingChaos.Ideation.Domain.Amendments.Events;
using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Persistence;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Command to apply an accepted amendment to an idea, creating a new version.
/// </summary>
/// <param name="IdeaId">The ID of the idea to amend.</param>
/// <param name="AmendmentId">The ID of the amendment being applied.</param>
/// <param name="NewTitle">The new title for the idea.</param>
/// <param name="NewBody">The new body content for the idea.</param>
/// <param name="ExpectedVersion">The expected version number of the idea (for optimistic concurrency).</param>
public sealed record AmendIdea(
    IdeaId IdeaId,
    AmendmentId AmendmentId,
    string NewTitle,
    string NewBody,
    int ExpectedVersion);

/// <summary>
/// Handles the application of an amendment to an idea.
/// </summary>
public static partial class AmendIdeaHandler
{
    /// <summary>
    /// Applies an amendment to an idea, creating a new version if the version matches.
    /// Publishes AmendmentApplicationFailed event if there is a version mismatch.
    /// </summary>
    /// <param name="command">The command containing amendment details.</param>
    /// <param name="ideaRepository">Repository for loading and saving ideas.</param>
    /// <param name="unitOfWork">Unit of work for committing changes.</param>
    /// <param name="messageBus">Message bus for publishing failure events.</param>
    /// <param name="logger">Logger for diagnostic output.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        AmendIdea command,
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var idea = await ideaRepository.GetByIdAsync(command.IdeaId, cancellationToken).ConfigureAwait(false);

        if (idea == null)
        {
            Logs.IdeaNotFound(logger, command.IdeaId.Value, command.AmendmentId.Value);

            await messageBus.PublishAsync(
                new AmendmentApplicationFailed(
                    command.AmendmentId.Value,
                    "IdeaNotFound",
                    null)).ConfigureAwait(false);
            return;
        }

        if (idea.CurrentVersion.VersionNumber != command.ExpectedVersion)
        {
            Logs.VersionMismatch(logger, command.IdeaId.Value, command.ExpectedVersion, idea.CurrentVersion.VersionNumber);

            await messageBus.PublishAsync(
                new AmendmentApplicationFailed(
                    command.AmendmentId.Value,
                    "VersionMismatch",
                    idea.CurrentVersion.VersionNumber)).ConfigureAwait(false);
            return;
        }

        idea.Amend(command.AmendmentId, command.NewTitle, command.NewBody);
        ideaRepository.Stage(idea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

        if (logger.IsEnabled(LogLevel.Information))
        {
            Logs.AmendmentApplied(logger, command.AmendmentId.Value, command.IdeaId.Value, idea.CurrentVersion.VersionNumber);
        }
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Warning, Message = "AmendIdea command failed: Idea {IdeaId} not found for amendment {AmendmentId}")]
        internal static partial void IdeaNotFound(ILogger logger, string ideaId, string amendmentId);

        [LoggerMessage(EventId = 2, Level = LogLevel.Warning, Message = "AmendIdea command failed: Version mismatch for idea {IdeaId}. Expected {ExpectedVersion}, actual {ActualVersion}")]
        internal static partial void VersionMismatch(ILogger logger, string ideaId, int expectedVersion, int actualVersion);

        [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Successfully applied amendment {AmendmentId} to idea {IdeaId}, creating version {NewVersion}")]
        internal static partial void AmendmentApplied(ILogger logger, string amendmentId, string ideaId, int newVersion);
    }
}
