using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Tagging.Application.ReadModels;
using BindingChaos.Tagging.Domain.Tags;
using Marten;

namespace BindingChaos.Tagging.Application.Services;

/// <summary>
/// Resolves existing tags by label or creates new ones.
/// </summary>
internal sealed class TagResolver(
    IQuerySession querySession,
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork) : ITagResolver
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Resolves existing tags or creates new tags for the specified labels.
    /// </summary>
    /// <param name="inputLabels">The labels to resolve or create.</param>
    /// <param name="originatorId">The participant initiating the operation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An array of <see cref="TagId"/> values for the resolved or created tags.</returns>
    public async Task<TagId[]> ResolveOrCreate(
        string[] inputLabels,
        ParticipantId originatorId,
        CancellationToken cancellationToken)
    {
        var requestedSlugs = inputLabels
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(TagSlugifier.Slugify)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (requestedSlugs.Length == 0)
        {
            return [];
        }

        List<TagId> tagIds = [];
        var existingTags = await querySession.Query<TagUsageView>()
            .Where(t => requestedSlugs.Contains(t.Slug))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var matchingExistingTags = existingTags.ToArray();

        tagIds.AddRange(matchingExistingTags.Select(t => TagId.Create(t.Id)));

        var existingSlugs = matchingExistingTags
            .Select(t => t.Slug)
            .ToHashSet(StringComparer.Ordinal);

        var missingSlugs = requestedSlugs
            .Where(slug => !existingSlugs.Contains(slug))
            .ToArray();

        foreach (var newTagSlug in missingSlugs)
        {
            var newTag = Tag.Create(newTagSlug, originatorId);
            tagIds.Add(newTag.Id);
            tagRepository.Stage(newTag);
        }

        if (missingSlugs.Length > 0)
        {
            await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        }

        return [.. tagIds];
    }
}
