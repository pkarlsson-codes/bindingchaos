using BindingChaos.Ideation.Domain.Ideas;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;

namespace BindingChaos.Ideation.Application.Commands;

/// <summary>
/// Represents an idea that is derived or "forked" from an existing context, including its title, description,
/// originator, society context, associated signals, and tags.
/// </summary>
/// <param name="Title">The title of the forked idea. This value cannot be null or empty.</param>
/// <param name="Body">The detailed description or content of the forked idea. This value cannot be null.</param>
/// <param name="AuthorId">The unique identifier of the participant who originated the forked idea.</param>
/// <param name="SocietyContext">The society under whose governance this idea runs.</param>
/// <param name="ParentIdeaId">The unique identifier of the parent idea from which this idea is forked.</param>
/// <param name="SignalReferences">A collection of identifiers representing the source signals that influenced the creation of this idea. This
/// collection cannot be null but may be empty.</param>
/// <param name="Tags">A collection of tags or keywords associated with the forked idea. This collection cannot be null but may be empty.</param>
public sealed record ForkIdea(
    string Title,
    string Body,
    ParticipantId AuthorId,
    SocietyId SocietyContext,
    IdeaId ParentIdeaId,
    string[] SignalReferences,
    string[] Tags);

/// <summary>
/// Provides functionality to handle the process of forking an idea by creating a new idea based on the specified
/// request.
/// </summary>
public static class ForkIdeaHandler
{
    /// <summary>Handles the <see cref="ForkIdea"/> command by creating a new forked idea aggregate.</summary>
    /// <param name="request">The fork command.</param>
    /// <param name="ideaRepository">Repository for staging the new idea.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The identifier of the newly forked idea.</returns>
    public static async Task<IdeaId> Handle(
        ForkIdea request,
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var idea = Idea.CreateFork(
            request.SocietyContext,
            request.AuthorId,
            request.Title,
            request.Body,
            request.ParentIdeaId,
            request.SignalReferences,
            request.Tags);

        ideaRepository.Stage(idea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return idea.Id;
    }
}
