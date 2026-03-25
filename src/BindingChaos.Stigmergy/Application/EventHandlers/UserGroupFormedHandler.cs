using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.Stigmergy.Domain.GoverningCommons;
using BindingChaos.Stigmergy.Domain.UserGroups.Events;
using Marten;

namespace BindingChaos.Stigmergy.Application.EventHandlers;

/// <summary>Activates a <see cref="Commons"/> when a user group is formed to govern it.</summary>
internal static class UserGroupFormedHandler
{
    /// <summary>Handles the <see cref="UserGroupFormed"/> message.</summary>
    /// <param name="message">The message to handle.</param>
    /// <param name="session">The document session.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task Handle(UserGroupFormed message, IDocumentSession session, CancellationToken cancellationToken)
    {
        var commonsId = CommonsId.Create(message.CommonsId);
        var commons = await session.LoadAsync<Commons>(commonsId, cancellationToken).ConfigureAwait(false)
            ?? throw new AggregateNotFoundException(typeof(Commons), message.CommonsId);

        commons.Activate();
        session.Store(commons);
        await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
