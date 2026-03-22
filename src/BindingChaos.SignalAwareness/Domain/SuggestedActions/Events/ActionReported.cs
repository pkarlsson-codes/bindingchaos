using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.SuggestedActions.Events;

/// <summary>
/// Represents a domain event that records the reporting of an action, including details about the action and its origin
/// within the aggregate.
/// </summary>
/// <param name="AggregateId">The unique identifier of the signal aggregate.</param>
/// <param name="ActionId">The unique identifier of the action that has been reported.</param>
/// <param name="ReportedBy">The identifier of the user or entity that reported the action.</param>
internal sealed record ActionReported(
    string AggregateId,
    string ActionId,
    string ReportedBy
) : DomainEvent(AggregateId);