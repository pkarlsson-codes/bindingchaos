using BindingChaos.CommunityDiscourse.Application.Commands;
using BindingChaos.Stigmergy.Contracts;
using Wolverine;

namespace BindingChaos.CommunityDiscourse.Infrastructure.EventHandlers;

/// <summary>
/// Wolverine message handler for <see cref="CommonsCreatedIntegrationEvent"/>.
/// Creates a discourse thread for the commons entity when a commons is proposed.
/// </summary>
/// <param name="messageBus">The mediator for sending commands.</param>
public sealed class CommonsCreatedHandler(IMessageBus messageBus)
{
    private readonly IMessageBus _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

    /// <summary>
    /// Handles the CommonsCreatedIntegrationEvent by creating a discourse thread via command.
    /// </summary>
    /// <param name="message">The incoming integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(CommonsCreatedIntegrationEvent message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var command = new CreateDiscourseThread("commons", message.CommonsId);
        await _messageBus.InvokeAsync(command).ConfigureAwait(false);
    }
}
