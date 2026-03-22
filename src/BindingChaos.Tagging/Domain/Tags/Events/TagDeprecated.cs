using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Domain.Tags.Events;

/// <summary>
/// Represents a deprecated tag event within the domain, including details about the reason and the user who
/// performed the action.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate associated with this event.</param>
/// <param name="Reason">The reason provided for deprecating the tag.</param>
/// <param name="PerformedById">The identifier of the user who performed the deprecation action.</param>
internal sealed record TagDeprecated(
    string AggregateId,
    string Reason,
    string PerformedById
) : DomainEvent(AggregateId);
