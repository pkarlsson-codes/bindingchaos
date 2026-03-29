using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Contracts;
using BindingChaos.Tagging.Application.Commands;
using BindingChaos.Tagging.Domain.TaggableTargets;
using Wolverine;

namespace BindingChaos.Tagging.Infrastructure.EventHandlers;

/// <summary>
/// Wolverine message handler for <see cref="IdeaDraftedIntegrationEvent"/>.
/// Creates a taggable target for the idea entity.
/// </summary>
public static class IdeaDraftedIntegrationHandler
{
    /// <summary>
    /// Handles the processing of an <see cref="IdeaDraftedIntegrationEvent"/> by creating the taggable target.
    /// </summary>
    /// <param name="message">The integration event containing details about the drafted idea.</param>
    /// <param name="messageBus">The message bus used for communication within the system.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task Handle(IdeaDraftedIntegrationEvent message, IMessageBus messageBus)
    {
        var targetId = TaggableTargetId.ForEntity(message.IdeaId);
        var authorId = ParticipantId.Create(message.AuthorId);

        var cmd = new CreateTaggableTarget(
            TargetId: targetId,
            ActorId: authorId,
            InitialTags: [],
            TagsOccurredAt: message.OccurredAt);

        await messageBus.InvokeAsync(cmd).ConfigureAwait(false);
    }
}
