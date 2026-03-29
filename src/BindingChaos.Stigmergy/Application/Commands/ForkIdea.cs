using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Ideas;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Fork idea command.
/// </summary>
/// <param name="AuthorId">Id of the author forking the idea.</param>
/// <param name="ParentIdeaId">Id of the idea that's being forked.</param>
/// <param name="Title">Title of the newly forked idea.</param>
/// <param name="Description">Description of the newly forked idea.</param>
public sealed record ForkIdea(ParticipantId AuthorId, IdeaId ParentIdeaId, string Title, string Description);

/// <summary>
/// <see cref="ForkIdea"/> command handler.
/// </summary>
public static class ForkIdeaHandler
{
    /// <summary>
    /// Handles a <see cref="ForkIdea"/> command.
    /// </summary>
    /// <param name="request">The command.</param>
    /// <param name="ideaRepository">An idea repository.</param>
    /// <param name="unitOfWork">A unit of work.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The id of the idea fork.</returns>
    public static async Task<IdeaId> Handle(
        ForkIdea request,
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var originalIdea = await ideaRepository
            .GetByIdOrThrowAsync(request.ParentIdeaId, cancellationToken)
            .ConfigureAwait(false);

        var forkedIdea = originalIdea.Fork(request.AuthorId, request.Title, request.Description);

        ideaRepository.Stage(forkedIdea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return forkedIdea.Id;
    }
}