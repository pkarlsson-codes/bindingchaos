using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Ideation.Application.ReadModels;
using BindingChaos.Ideation.Domain;
using BindingChaos.Ideation.Domain.Ideas;

namespace BindingChaos.CorePlatform.API.Mappings;

/// <summary>Pure structural mapping from idea read models to API response contracts.</summary>
internal static class IdeaMapper
{
    /// <summary>Maps a <see cref="IdeaView"/> to a <see cref="IdeaResponse"/>.</summary>
    /// <param name="idea">The idea read model to map.</param>
    /// <param name="sourceSignals">The pre-resolved source signal titles for this idea.</param>
    /// <param name="authorPseudonym">The pseudonym for the idea's author.</param>
    /// <returns>The mapped <see cref="IdeaResponse"/>.</returns>
    internal static IdeaResponse ToIdeaResponse(IdeaView idea, IReadOnlyList<IdeaSourceSignal> sourceSignals, string authorPseudonym)
    {
        return new IdeaResponse(
            idea.Id,
            idea.CurrentTitle,
            idea.CurrentBody,
            idea.SocietyContext,
            [.. sourceSignals],
            authorPseudonym,
            OpenAmendmentCount: 0,
            idea.CreatedAt,
            idea.LastUpdatedAt,
            [.. idea.Tags],
            IdeaStatus.FromValue(idea.Status).DisplayName);
    }

    /// <summary>Maps a <see cref="IdeasListItemView"/> to a <see cref="IdeaListItemResponse"/>.</summary>
    /// <param name="idea">The list-item read model to map.</param>
    /// <returns>The mapped <see cref="IdeaListItemResponse"/>.</returns>
    internal static IdeaListItemResponse ToIdeaListItemResponse(IdeasListItemView idea)
    {
        return new IdeaListItemResponse(
            idea.Id,
            idea.Title,
            idea.Body,
            idea.SocietyContext,
            idea.SourceSignalIds,
            idea.OpenAmendmentCount,
            idea.CreatedAt,
            idea.LastUpdatedAt,
            idea.Tags,
            IdeaStatus.Published.DisplayName);
    }
}
