using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons.Events;

/// <summary>Raised when a <see cref="Commons"/> transitions from Proposed to Active.</summary>
internal sealed record CommonsActivated(string AggregateId) : DomainEvent(AggregateId);
