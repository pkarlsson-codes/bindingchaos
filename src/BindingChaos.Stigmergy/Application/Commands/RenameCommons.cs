using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.GoverningCommons;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Renames an existing <see cref="Commons"/>.</summary>
/// <param name="CommonsId">The ID of the commons to rename.</param>
/// <param name="NewName">The new name for the commons.</param>
public sealed record RenameCommons(CommonsId CommonsId, string NewName);

/// <summary>Handles the <see cref="RenameCommons"/> command.</summary>
public static class RenameCommonsHandler
{
    /// <summary>Handles the <see cref="RenameCommons"/> command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="commonsRepository">The repository to retrieve and persist the commons.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task Handle(
        RenameCommons command,
        ICommonsRepository commonsRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commons = await commonsRepository.GetByIdOrThrowAsync(command.CommonsId, cancellationToken).ConfigureAwait(false);

        commons.Rename(command.NewName);

        commonsRepository.Stage(commons);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
    }
}
