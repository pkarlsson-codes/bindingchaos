using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.Ideas;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>
/// Update idea draft command.
/// </summary>
/// <param name="IdeaId">Id of the idea to update.</param>
/// <param name="ActorId">Id of actor updating the idea.</param>
/// <param name="Title">New idea title.</param>
/// <param name="Description">New idea descirption.</param>
public sealed record UpdateIdeaDraft(IdeaId IdeaId, ParticipantId ActorId, string Title, string Description);

/// <summary>
/// <see cref="UpdateIdeaDraft"/> command handler.
/// </summary>
public static class UpdateIdeaDraftHandler
{
    /// <summary>
    /// Handles an <see cref="UpdateIdeaDraft"/> command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="ideaRepository">An idea repository.</param>
    /// <param name="unitOfWork">A unit of work.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task.</returns>
    public static async Task Handle(
        UpdateIdeaDraft command,
        IIdeaRepository ideaRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var idea = await ideaRepository
            .GetByIdOrThrowAsync(command.IdeaId, cancellationToken)
            .ConfigureAwait(false);

        idea.Update(command.ActorId, command.Title, command.Description);
        ideaRepository.Stage(idea);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}