using BindingChaos.SharedKernel.Domain;
using BindingChaos.SignalAwareness.Contracts;
using BindingChaos.Tagging.Application.Commands;
using BindingChaos.Tagging.Domain.TaggableTargets;
using Wolverine;

namespace BindingChaos.Tagging.Infrastructure.EventHandlers;

/// <summary>
/// Wolverine message handler for <see cref="SignalCapturedIntegrationEvent"/>.
/// Creates a taggable target for the signal entity and assigns its initial tags atomically.
/// </summary>
public static class SignalCapturedHandler
{
    /// <summary>
    /// Handles the processing of a signal captured integration event by creating the taggable target
    /// and assigning its initial tags in a single operation.
    /// </summary>
    /// <param name="message">The integration event containing the signal details, including the target, originator, tags, and timestamp.</param>
    /// <param name="messageBus">The message bus used to invoke the create command.</param>
    /// <returns>A task that represents the asynchronous operation of handling the event.</returns>
    public static async Task Handle(SignalCapturedIntegrationEvent message, IMessageBus messageBus)
    {
        var targetId = TaggableTargetId.ForEntity(message.SignalId);
        var originatorId = ParticipantId.Create(message.OriginatorId);

        var cmd = new CreateTaggableTarget(
            TargetId: targetId,
            ActorId: originatorId,
            InitialTags: message.Tags ?? [],
            TagsOccurredAt: message.OccurredAt);

        await messageBus.InvokeAsync(cmd).ConfigureAwait(false);
    }
}
