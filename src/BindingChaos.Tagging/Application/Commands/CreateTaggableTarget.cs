using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Tagging.Application.Services;
using BindingChaos.Tagging.Domain.TaggableTargets;

namespace BindingChaos.Tagging.Application.Commands;

/// <summary>
/// Represents an operation in which a participant identifies a taggable target.
/// </summary>
/// <param name="TargetId">The unique identifier of the target to be identified.</param>
/// <param name="ActorId">The unique identifier of the participant performing the identification.</param>
/// <param name="InitialTags">Optional initial tag labels to assign atomically with creation.</param>
/// <param name="TagsOccurredAt">The timestamp to use for the initial tag assignment. Required when <paramref name="InitialTags"/> is non-empty.</param>
public sealed record CreateTaggableTarget(
    TaggableTargetId TargetId,
    ParticipantId ActorId,
    IReadOnlyList<string>? InitialTags = null,
    DateTimeOffset? TagsOccurredAt = null
);

/// <summary>
/// Handles the creation of a new taggable target if one does not already exist.
/// </summary>
public static class CreateTaggableTargetHandler
{
    /// <summary>
    /// Creates a new taggable target if one with the specified identifier does not already exist,
    /// optionally assigning initial tags in the same transaction.
    /// </summary>
    /// <param name="cmd">The command containing the target identifier to create.</param>
    /// <param name="taggableTargetRepository">The repository for taggable target aggregates.</param>
    /// <param name="tagResolver">Resolves or creates tags by label.</param>
    /// <param name="unitOfWork">The unit of work for persisting changes.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task Handle(
        CreateTaggableTarget cmd,
        ITaggableTargetRepository taggableTargetRepository,
        ITagResolver tagResolver,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cmd);

        var exists = await taggableTargetRepository.ExistsByIdAsync(cmd.TargetId, cancellationToken).ConfigureAwait(false);

        if (!exists)
        {
            var newTarget = new TaggableTarget(cmd.TargetId);

            var processedLabels = (cmd.InitialTags ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (processedLabels.Length > 0)
            {
                var tagIds = await tagResolver.ResolveOrCreate(
                    processedLabels,
                    cmd.ActorId,
                    cancellationToken).ConfigureAwait(false);

                newTarget.AssignTags([.. tagIds.Distinct()], cmd.ActorId);
            }

            taggableTargetRepository.Stage(newTarget);
            await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
