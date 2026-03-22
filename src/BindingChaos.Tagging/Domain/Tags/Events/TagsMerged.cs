using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Domain.Tags.Events;

/// <summary>
/// Represents a domain event indicating that tags have been merged.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate associated with this event.</param>
/// <param name="TargetTagId">The identifier of the tag that remains after the merge.</param>
/// <param name="CarryOverAliasSlugs">A read-only list of alias slugs that were carried over to the target tag.</param>
/// <param name="PerformedById">The identifier of the user who performed the tag merge operation.</param>
internal sealed record TagsMerged(
    string AggregateId,
    string TargetTagId,
    IReadOnlyList<string> CarryOverAliasSlugs,
    string PerformedById
) : DomainEvent(AggregateId);
