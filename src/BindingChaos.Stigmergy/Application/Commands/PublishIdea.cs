using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Ideas;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Publish idea command.
/// </summary>
/// <param name="IdeaId">Id of the <see cref="Idea"/> to publish.</param>
/// <param name="ActorId">Id of the actor publishing the idea.</param>
public sealed record PublishIdea(IdeaId IdeaId, ParticipantId ActorId);

/// <summary>
/// <see cref="PublishIdea"/> command handler.
/// </summary>
public static class PublishIdeaHandler
{
    /// <summary>
    /// Handles a <see cref="PublishIdea"/> command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ideaRepository">An idea repository.</param>
    /// <param name="unitOfWork">A unit of work.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        PublishIdea command,
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var idea = await ideaRepository
            .GetByIdOrThrowAsync(command.IdeaId, cancellationToken)
            .ConfigureAwait(false);
        idea.Publish(command.ActorId);

        ideaRepository.Stage(idea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}