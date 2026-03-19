using BindingChaos.Ideation.Contracts;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Tagging.Application.Commands;
using BindingChaos.Tagging.Domain.TaggableTargets;
using Wolverine;

namespace BindingChaos.Tagging.Infrastructure.EventHandlers;

/// <summary>
/// Wolverine message handler for <see cref="IdeaAuthoredIntegrationEvent"/>.
/// Creates a taggable target for the idea entity and assigns its initial tags atomically.
/// </summary>
public static class IdeaAuthoredIntegrationHandler
{
    /// <summary>
    /// Handles the processing of an <see cref="IdeaAuthoredIntegrationEvent"/> by creating the taggable target
    /// and assigning its initial tags in a single operation.
    /// </summary>
    /// <param name="message">The integration event containing details about the authored idea, including its ID, author, tags, and society
    /// context.</param>
    /// <param name="messageBus">The message bus used for communication within the system.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async Task Handle(IdeaAuthoredIntegrationEvent message, IMessageBus messageBus)
    {
        var targetId = TaggableTargetId.ForEntity(message.IdeaId);
        var authorId = ParticipantId.Create(message.AuthorId);

        var cmd = new CreateTaggableTarget(
            TargetId: targetId,
            ActorId: authorId,
            InitialTags: message.Tags ?? [],
            TagsOccurredAt: message.AuthoredAt);

        await messageBus.InvokeAsync(cmd).ConfigureAwait(false);
    }
}
