using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons.Events;

/// <summary>Domain event raised when a Commons is proposed.</summary>
/// <param name="AggregateId">The unique identifier of the commons.</param>
/// <param name="Name">The name of the commons.</param>
/// <param name="Description">The description of the commons.</param>
/// <param name="FounderId">The identifier of the participant who proposed the commons.</param>
public sealed record CommonsCreated(
    string AggregateId,
    string Name,
    string Description,
    string FounderId)
    : DomainEvent(AggregateId);