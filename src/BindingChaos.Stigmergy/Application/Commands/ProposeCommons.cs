using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using Marten;
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
    /// <param name="session">The document session.</param>
    /// <param name="messageBus">The message bus.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The ID of the proposed <see cref="Commons"/>.</returns>
    public static async Task<CommonsId> Handle(
        ProposeCommons command,
        IDocumentSession session,
        IMessageBus messageBus,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commons = Commons.Propose(command.Name, command.Description, command.FounderId);
        session.Store(commons);
        await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return commons.Id;
    }
}