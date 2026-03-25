using BindingChaos.CommunityDiscourse.Application.Commands;
using BindingChaos.Ideation.Contracts;
using Wolverine;

namespace BindingChaos.CommunityDiscourse.Infrastructure.EventHandlers;

/// <summary>
/// Wolverine message handler for <see cref="IdeaAuthoredIntegrationEvent"/>.
/// Creates a discourse thread for the idea entity if it doesn't already exist.
/// </summary>
/// <param name="messageBus">The message bus for handling asynchronous messaging.</param>
public sealed class IdeaAuthoredHandler(IMessageBus messageBus)
{
    private readonly IMessageBus _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

    /// <summary>
    /// Handles the IdeaAuthoredIntegrationEvent by creating a discourse thread via command.
    /// </summary>
    /// <param name="message">The incoming integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(IdeaAuthoredIntegrationEvent message)
    {
        var command = new CreateDiscourseThread("idea", message.IdeaId);
        await _messageBus.InvokeAsync(command).ConfigureAwait(false);
    }
}