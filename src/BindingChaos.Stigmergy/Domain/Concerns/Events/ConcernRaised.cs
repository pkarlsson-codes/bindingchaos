using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Concerns.Events;

/// <summary>
/// Concern raised event.
/// </summary>
/// <param name="AggregateId">Id of the raised concern.</param>
/// <param name="ActorId">Id of the actor raising the concern.</param>
/// <param name="Name">Name of the raised concern.</param>
/// <param name="Tags">Tags associated with the raised concern.</param>
/// <param name="SignalIds">Ids of signals revealing the concern.</param>
/// <param name="Origin">How the concern came to be raised.</param>
/// <param name="ClusterId">Id of the signal cluster, when origin is <see cref="ConcernOrigin.EmergingPattern"/>.</param>
public sealed record ConcernRaised(
    string AggregateId,
    string ActorId,
    string Name,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> SignalIds,
    ConcernOrigin Origin,
    string? ClusterId
) : DomainEvent(AggregateId);