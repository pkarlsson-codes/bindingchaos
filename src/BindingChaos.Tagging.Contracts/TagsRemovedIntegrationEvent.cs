using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Contracts;

/// <summary>
/// Represents an event that occurs when one or more tags are removed from a target entity within a specific context.
/// </summary>
/// <param name="EntityId">The original entity ID (e.g. <c>idea-abc123</c>) from which tags have been removed.</param>
/// <param name="TagIds">An array containing the unique identifiers of the tags that were removed from the target entity.</param>
public sealed record TagsRemovedIntegrationEvent(
    string EntityId,
    string[] TagIds
) : IntegrationEvent;