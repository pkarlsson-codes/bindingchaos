using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Contracts;

/// <summary>
/// Concern raised integration event.
/// </summary>
/// <param name="ConcernId">Id of the raised concern.</param>
/// <param name="ActorId">Id of the actor that raised the concern.</param>
/// <param name="Name">Name of the raised concern.</param>
public sealed record ConcernRaisedIntegrationEvent(
    string ConcernId,
    string ActorId,
    string Name
) : IntegrationEvent;