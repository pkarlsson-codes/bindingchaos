using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Ideas;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Command to draft an idea.
/// </summary>
/// <param name="AuthorId">Id of the idea author.</param>
/// <param name="Title">Title of the idea.</param>
/// <param name="Description">Description of the idea.</param>
public sealed record DraftIdea(ParticipantId AuthorId, string Title, string Description);

/// <summary>
/// <see cref="DraftIdea"/> command handler.
/// </summary>
public static class DraftIdeaHandler
{
    /// <summary>
    /// Handles a <see cref="DraftIdea"/> command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ideaRepository">An idea repository.</param>
    /// <param name="unitOfWork">A unit of work.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>Id of the drafted idea.</returns>
    public static async Task<IdeaId> Handle(
        DraftIdea command,
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var idea = Idea.Draft(command.AuthorId, command.Title, command.Description);

        ideaRepository.Stage(idea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return idea.Id;
    }
}