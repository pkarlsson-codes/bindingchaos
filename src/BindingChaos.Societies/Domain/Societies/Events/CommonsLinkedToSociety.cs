using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Societies.Domain.Societies.Events;

/// <summary>
/// Raised when a society declares that a commons affects its members.
/// </summary>
/// <param name="AggregateId">The ID of the society.</param>
/// <param name="CommonsId">The ID of the commons.</param>
/// <param name="DeclaredBy">The participant who made the declaration.</param>
public sealed record CommonsLinkedToSociety(
    string AggregateId,
    string CommonsId,
    string DeclaredBy
) : DomainEvent(AggregateId);
