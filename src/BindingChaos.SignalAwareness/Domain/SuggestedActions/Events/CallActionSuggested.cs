using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.SuggestedActions.Events;

/// <summary>
/// Raised when a participant suggests a 'make a call' action on a signal.
/// </summary>
/// <param name="AggregateId">The unique identifier of the signal.</param>
/// <param name="Version">The version number of the event within the aggregate's event stream.</param>
/// <param name="SignalId">Id of the <see cref="Signals.Signal"/> the action is for.</param>
/// <param name="PhoneNumber">The phone number to call.</param>
/// <param name="Details">Optional free-text context about the call.</param>
/// <param name="SuggestedBy">The identifier of the participant who made the suggestion.</param>
internal sealed record CallActionSuggested(
    string AggregateId,
    long Version,
    string SignalId,
    string PhoneNumber,
    string? Details,
    string SuggestedBy
) : DomainEvent(AggregateId, Version);
