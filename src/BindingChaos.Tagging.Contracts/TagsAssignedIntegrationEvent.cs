using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Contracts;

/// <summary>
/// Represents an integration event that occurs when one or more tags are assigned to a target entity within a specified
/// context.
/// </summary>
/// <param name="EntityId">The original entity ID (e.g. <c>idea-abc123</c>) to which the tags are assigned.</param>
/// <param name="TagIds">An array of tags that are assigned to the target entity. Each tag is represented as a string.</param>
public sealed record TagsAssignedIntegrationEvent(
    string EntityId,
    string[] TagIds
) : IntegrationEvent;
