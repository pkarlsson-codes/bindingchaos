using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Domain.SuggestedActions.Events;

/// <summary>
/// Raised when a participant suggests visiting a webpage as an action on a signal.
/// </summary>
/// <param name="AggregateId">The unique identifier of the signal.</param>
/// <param name="Version">The version number of the event within the aggregate's event stream.</param>
/// <param name="SignalId">The unique identifier of the signal the action is for.</param>
/// <param name="Url">The URL to visit.</param>
/// <param name="Details">Optional free-text context about what to do on the page.</param>
/// <param name="SuggestedBy">The identifier of the participant who made the suggestion.</param>
internal sealed record WebpageActionSuggested(
    string AggregateId,
    long Version,
    string SignalId,
    string Url,
    string? Details,
    string SuggestedBy
) : DomainEvent(AggregateId, Version);
