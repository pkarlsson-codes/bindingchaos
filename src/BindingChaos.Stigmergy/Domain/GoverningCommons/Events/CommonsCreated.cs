using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons.Events;

internal sealed record CommonsCreated(
    string AggregateId,
    string Name,
    string Description,
    string FounderId)
    : DomainEvent(AggregateId);