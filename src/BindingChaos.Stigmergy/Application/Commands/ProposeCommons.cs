using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Wolverine;

namespace BindingChaos.Stigmergy.Application.Commands;

/// <summary>Proposes a new <see cref="Domain.GoverningCommons.Commons"/>.</summary>
/// <param name="Name">Name of the commons.</param>
/// <param name="Description">Description of the commons.</param>
/// <param name="FounderId">The participant proposing the commons.</param>
public sealed record ProposeCommons(string Name, string Description, ParticipantId FounderId);

/// <summary>
/// Handles the <see cref="ProposeCommons"/> command.
/// </summary>
public static class ProposeCommonsHandler
{
    /// <summary>Handles the <see cref="ProposeCommons"/> command.</summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="commonsRepository">The repository to persist the commons.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the proposed <see cref="Commons"/>.</returns>
    public static async Task<CommonsId> Handle(
        ProposeCommons command,
        ICommonsRepository commonsRepository,
        IUnitOfWork unitOfWork,
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commons = Commons.Propose(command.Name, command.Description, command.FounderId);
        commonsRepository.Stage(commons);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return commons.Id;
    }
}
