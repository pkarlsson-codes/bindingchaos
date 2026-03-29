using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Concerns.Events;

/// <summary>
/// Concern raised event.
/// </summary>
/// <param name="AggregateId">Id of the raised concern.</param>
/// <param name="ActorId">Id of the actor raising the concern.</param>
/// <param name="Name">Name of the raised concern.</param>
/// <param name="SignalIds">Ids of signals revealing the concern.</param>
public sealed record ConcernRaised(
    string AggregateId,
    string ActorId,
    string Name,
    IReadOnlyList<string> SignalIds
) : DomainEvent(AggregateId);