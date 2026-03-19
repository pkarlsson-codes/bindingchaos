using BindingChaos.CommunityDiscourse.Application.Commands;
using BindingChaos.Ideation.Contracts;
using Wolverine;

namespace BindingChaos.CommunityDiscourse.Infrastructure.EventHandlers;

/// <summary>
/// Wolverine message handler for <see cref="AmendmentProposedIntegrationEvent"/>.
/// Creates a discourse thread for the amendment entity if it doesn't already exist.
/// </summary>
/// <param name="messageBus">The message bus for handling asynchronous messaging.</param>
public class AmendmentProposedHandler(IMessageBus messageBus)
{
    private readonly IMessageBus _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

    /// <summary>
    /// Handles the AmendmentProposedIntegrationEvent by creating a discourse thread via command.
    /// </summary>
    /// <param name="message">The incoming integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(AmendmentProposedIntegrationEvent message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var command = new CreateDiscourseThread("amendment", message.AmendmentId);
        await _messageBus.InvokeAsync(command).ConfigureAwait(false);
    }
}
