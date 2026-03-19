using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Represents a command to author a new idea with a title, description, and associated metadata.
/// </summary>
/// <param name="Title">The title of the idea. This value cannot be null or empty.</param>
/// <param name="Description">A detailed description of the idea. This value cannot be null or empty.</param>
/// <param name="OriginatorId">The unique identifier of the participant who originated the idea. This value cannot be null.</param>
/// <param name="SocietyContext">The society under whose governance this idea runs. This value cannot be null.</param>
/// <param name="SourceSignalId">A collection of identifiers representing the source signals that inspired the idea. This collection cannot be null
/// but may be empty.</param>
/// <param name="Tags">A collection of tags categorizing the idea. This collection cannot be null but may be empty.</param>
public sealed record AuthorIdea(
    string Title, string Description,
    ParticipantId OriginatorId, SocietyId SocietyContext,
    string[] SourceSignalId, string[] Tags);

/// <summary>
/// Handles creation of a new Idea aggregate.
/// </summary>
public static class AuthorIdeaHandler
{
    /// <summary>Handles the <see cref="AuthorIdea"/> command by creating and persisting a new Idea aggregate.</summary>
    /// <param name="request">The author idea command.</param>
    /// <param name="ideaRepository">Repository for staging the new idea.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The identifier of the newly authored idea.</returns>
    public static async Task<IdeaId> Handle(
        AuthorIdea request,
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var idea = Idea.Author(
            request.SocietyContext,
            request.OriginatorId,
            request.Title,
            request.Description,
            request.SourceSignalId,
            request.Tags);

        ideaRepository.Stage(idea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return idea.Id;
    }
}
