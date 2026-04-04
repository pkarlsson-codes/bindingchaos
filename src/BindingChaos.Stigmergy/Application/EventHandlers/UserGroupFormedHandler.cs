using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;

namespace BindingChaos.Stigmergy.Application.EventHandlers;

/// <summary>Activates a <see cref="Commons"/> when a user group is formed to govern it.</summary>
internal static class UserGroupFormedHandler
{
    /// <summary>Handles the <see cref="UserGroupFormed"/> message.</summary>
    /// <param name="message">The message to handle.</param>
    /// <param name="commonsRepository">The repository to retrieve and persist the commons.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task Handle(UserGroupFormed message, ICommonsRepository commonsRepository, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        var commonsId = CommonsId.Create(message.CommonsId);
        var commons = await commonsRepository.GetByIdOrThrowAsync(commonsId, cancellationToken).ConfigureAwait(false);

        if (!commons.IsProposed)
        {
            return;
        }

        commons.Activate();
        commonsRepository.Stage(commons);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
