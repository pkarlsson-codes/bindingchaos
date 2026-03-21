using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SignalAwareness.Domain.Signals;
using BindingChaos.SignalAwareness.Domain.SuggestedActions.Events;

namespace BindingChaos.SignalAwareness.Domain.SuggestedActions;

/// <summary>
/// Represents a structured action suggested by a participant on a signal.
/// </summary>
public sealed class SuggestedAction : AggregateRoot<SuggestedActionId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuggestedAction"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of this suggested action.</param>
    /// <param name="signalId">The unique identifier of the signal that the action is for.</param>
    /// <param name="parameters">The structured parameters describing the action to take.</param>
    /// <param name="suggestedBy">The identifier of the participant who made the suggestion.</param>
    public SuggestedAction(SuggestedActionId id, SignalId signalId, MakeACallParameters parameters, ParticipantId suggestedBy)
    {
        this.ApplyChange(new CallActionSuggested(id.Value, 0, signalId.Value, parameters.PhoneNumber, parameters.Details, suggestedBy.Value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SuggestedAction"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of this suggested action.</param>
    /// <param name="signalId">The unique identified of the <see cref="Signal"/> the action is for.</param>
    /// <param name="parameters">The structured parameters describing the action to take.</param>
    /// <param name="suggestedBy">The identifier of the participant who made the suggestion.</param>
    public SuggestedAction(SuggestedActionId id, SignalId signalId, VisitAWebpageParameters parameters, ParticipantId suggestedBy)
    {
        this.ApplyChange(new WebpageActionSuggested(id.Value, 0, signalId.Value, parameters.Url, parameters.Details, suggestedBy.Value));
    }

    /// <summary>
    /// Initializes a new instance of the SuggestedAction class.
    /// </summary>
    public SuggestedAction()
    {
    }

    /// <summary>
    /// Creates a suggested action to initiate a phone call to the specified phone number.
    /// </summary>
    /// <param name="signalId">The unique identifier of the signal that the action is for.</param>
    /// <param name="phoneNumber">The phone number to call. Must be in a valid phone number format.</param>
    /// <param name="details">Optional additional information about the call. Specify null if no details are provided.</param>
    /// <param name="suggestedBy">The identifier of the participant who is suggesting the call.</param>
    /// <returns>A SuggestedAction object that represents the action to make a call, including the provided parameters.</returns>
    public static SuggestedAction SuggestMakeACall(SignalId signalId, string phoneNumber, string? details, ParticipantId suggestedBy)
    {
        return new SuggestedAction(SuggestedActionId.Generate(), signalId, new MakeACallParameters(phoneNumber, details), suggestedBy);
    }

    /// <summary>
    /// Creates a suggested action that prompts a participant to visit a specified website.
    /// </summary>
    /// <param name="signalId">The identifier of the signal associated with the suggested action.</param>
    /// <param name="url">The URL of the website to be visited.</param>
    /// <param name="details">Optional additional information about the suggested action. May be null.</param>
    /// <param name="suggestedBy">The identifier of the participant who is suggesting the action.</param>
    /// <returns>A SuggestedAction instance representing the request to visit the specified website.</returns>
    internal static SuggestedAction SuggestVisitAWebsite(SignalId signalId, string url, string? details, ParticipantId suggestedBy)
    {
        return new SuggestedAction(SuggestedActionId.Generate(), signalId, new VisitAWebpageParameters(url, details), suggestedBy);
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case CallActionSuggested e: Apply(e); break;
            case WebpageActionSuggested e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(CallActionSuggested e)
    {
        Id = SuggestedActionId.Create(e.AggregateId);
    }

    private void Apply(WebpageActionSuggested e)
    {
        Id = SuggestedActionId.Create(e.AggregateId);
    }
}
