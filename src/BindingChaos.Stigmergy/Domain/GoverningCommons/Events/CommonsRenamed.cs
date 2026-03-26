using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.GoverningCommons.Events;

/// <summary>Raised when a <see cref="Commons"/> is renamed.</summary>
/// <param name="AggregateId">The unique identifier of the commons.</param>
/// <param name="NewName">The new name of the commons.</param>
public sealed record CommonsRenamed(string AggregateId, string NewName) : DomainEvent(AggregateId);
