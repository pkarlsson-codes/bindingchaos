using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.Stigmergy.Application.ReadModels;

namespace BindingChaos.CorePlatform.API.Mappings;

/// <summary>Pure structural mapping from idea read models to API response contracts.</summary>
internal static class IdeaMapper
{
    /// <summary>Maps a <see cref="IdeaView"/> to a <see cref="IdeaResponse"/>.</summary>
    /// <param name="idea">The idea read model to map.</param>
    /// <param name="authorPseudonym">The pseudonym for the idea's author.</param>
    /// <returns>The mapped <see cref="IdeaResponse"/>.</returns>
    internal static IdeaResponse ToIdeaResponse(IdeaView idea, string authorPseudonym)
    {
        return new IdeaResponse(
            idea.Id,
            idea.Title,
            idea.Description,
            authorPseudonym,
            idea.CreatedAt,
            idea.LastUpdatedAt,
            idea.Status);
    }

    /// <summary>Maps a <see cref="IdeasListItemView"/> to a <see cref="IdeaListItemResponse"/>.</summary>
    /// <param name="idea">The list-item read model to map.</param>
    /// <returns>The mapped <see cref="IdeaListItemResponse"/>.</returns>
    internal static IdeaListItemResponse ToIdeaListItemResponse(IdeasListItemView idea)
    {
        return new IdeaListItemResponse(
            idea.Id,
            idea.Title,
            idea.Description,
            idea.CreatedAt,
            idea.LastUpdatedAt,
            idea.Status);
    }
}
