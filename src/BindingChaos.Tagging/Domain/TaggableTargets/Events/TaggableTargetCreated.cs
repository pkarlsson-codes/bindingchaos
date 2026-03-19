using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Tagging.Domain.TaggableTargets.Events;

/// <summary>
/// Represents an event that occurs when a taggable target is created.
/// </summary>
/// <param name="AggregateId">The unique identifier of the aggregate associated with the taggable target.</param>
/// <param name="Version">The version number of the taggable target at the time of creation.</param>
public sealed record TaggableTargetCreated(
    string AggregateId,
    long Version
    ) : DomainEvent(AggregateId, Version);
