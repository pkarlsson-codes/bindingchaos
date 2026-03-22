using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Domain.Tags.Events;

/// <summary>
/// Represents an event that occurs when a new tag is created.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate associated with the tag.</param>
/// <param name="PreferredLabel">The preferred human-readable label for the tag.</param>
/// <param name="PreferredSlug">The preferred slug for the tag, typically used in URLs or identifiers.</param>
/// <param name="CreatedById">The unique identifier of the user who created the tag.</param>
internal sealed record TagCreated(
    string AggregateId,
    string PreferredLabel,
    string PreferredSlug,
    string CreatedById
) : DomainEvent(AggregateId);
