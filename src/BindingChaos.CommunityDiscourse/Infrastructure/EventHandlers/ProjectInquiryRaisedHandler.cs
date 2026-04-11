using BindingChaos.CommunityDiscourse.Application.Commands;
using BindingChaos.Stigmergy.Contracts;
using Wolverine;

namespace BindingChaos.CommunityDiscourse.Infrastructure.EventHandlers;

/// <summary>
/// Wolverine message handler for <see cref="ProjectInquiryRaisedIntegrationEvent"/>.
/// Creates a discourse thread for the inquiry so community members can discuss it.
/// </summary>
/// <param name="messageBus">The message bus for handling asynchronous messaging.</param>
public sealed class ProjectInquiryRaisedHandler(IMessageBus messageBus)
{
    private readonly IMessageBus _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));

    /// <summary>
    /// Handles the <see cref="ProjectInquiryRaisedIntegrationEvent"/> by creating a discourse thread.
    /// </summary>
    /// <param name="message">The incoming integration event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(ProjectInquiryRaisedIntegrationEvent message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var command = new CreateDiscourseThread("project_inquiry", message.InquiryId);
        await _messageBus.InvokeAsync(command).ConfigureAwait(false);
    }
}
