using BindingChaos.CommunityDiscourse.Application.Commands;
using BindingChaos.Stigmergy.Contracts;
using Wolverine;

namespace BindingChaos.CommunityDiscourse.Infrastructure.EventHandlers;

/// <summary>
/// Wolverine message handler for <see cref="SignalCapturedIntegrationEvent"/>.
/// Creates a discourse thread for the signal entity if it doesn't already exist.
/// </summary>
/// <param name="messageBus">The mediator for sending commands.</param>
public sealed class SignalCapturedHandler(IMessageBus messageBus)
{
    private readonly IMessageBus _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

    /// <summary>
    /// Handles the SignalCapturedIntegrationEvent by creating a discourse thread via command.
    /// </summary>
    /// <param name="message">The incoming integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(SignalCapturedIntegrationEvent message)
    {
        ArgumentNullException.ThrowIfNull(message, nameof(message));

        var command = new CreateDiscourseThread("signal", message.SignalId);
        await _messageBus.InvokeAsync(command).ConfigureAwait(false);
    }
}
