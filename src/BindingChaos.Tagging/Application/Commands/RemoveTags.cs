using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Tagging.Application.Services;
using BindingChaos.Tagging.Domain.TaggableTargets;

namespace BindingChaos.Tagging.Application.Commands;

/// <summary>
/// Remove tags from a target (Idea/Signal). Meant to be invoked from integration events.
/// </summary>
public sealed record RemoveTags(
    TaggableTargetId TargetId,
    ParticipantId ActorId,
    IReadOnlyList<string> Labels,
    DateTimeOffset OccurredAt
);

/// <summary>
/// Handles the removal of specified tags from a target entity.
/// </summary>
public static class RemoveTagsHandler
{
    /// <summary>
    /// Removes the specified tags from the target, resolving or creating tag records as needed.
    /// </summary>
    /// <param name="cmd">The command containing the target, actor, and labels to remove.</param>
    /// <param name="tagResolver">Resolves or creates tags by label within a locality.</param>
    /// <param name="taggableTargetRepository">The repository for taggable target aggregates.</param>
    /// <param name="unitOfWork">The unit of work for persisting changes.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task Handle(
        RemoveTags cmd,
        ITagResolver tagResolver,
        ITaggableTargetRepository taggableTargetRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(cmd);

        var processedLabels = (cmd.Labels ?? [])
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (processedLabels.Length == 0)
        {
            return;
        }

        var target = await taggableTargetRepository.GetByIdOrThrowAsync(cmd.TargetId, cancellationToken).ConfigureAwait(false);

        var tagIds = await tagResolver.ResolveOrCreate(processedLabels, cmd.ActorId, cancellationToken).ConfigureAwait(false);

        target.RemoveTags([.. tagIds.Distinct()], cmd.ActorId);

        taggableTargetRepository.Stage(target);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
