using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Domain.TaggableTargets.Events;

/// <summary>
/// Represents an event that occurs when a taggable target is created.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate associated with the taggable target.</param>
public sealed record TaggableTargetCreated(
    string AggregateId
    ) : DomainEvent(AggregateId);
