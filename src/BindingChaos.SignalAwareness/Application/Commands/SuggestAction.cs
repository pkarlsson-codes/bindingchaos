using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Persistence;
using BindingChaos.SignalAwareness.Domain.Signals;
using BindingChaos.SignalAwareness.Domain.SuggestedActions;

namespace BindingChaos.SignalAwareness.Application.Commands;

/// <summary>
/// Represents a structured action suggestion submitted by a participant on a signal.
/// </summary>
/// <param name="SignalId">The unique identifier of the signal.</param>
/// <param name="SuggestedBy">The unique identifier of the participant suggesting the action.</param>
/// <param name="Parameters">
/// The typed parameters describing the action. The concrete subtype determines the action type.
/// </param>
public sealed record SuggestAction(SignalId SignalId, ParticipantId SuggestedBy, ActionParameters Parameters);

/// <summary>Handles the <see cref="SuggestAction"/> command.</summary>
public static class SuggestActionHandler
{
    /// <summary>Records a suggested action on a signal.</summary>
    /// <param name="request">The suggest action command.</param>
    /// <param name="signalRepository">Repository for loading and staging the signal.</param>
    /// <param name="signalActionRepository">Repository for <see cref="SuggestedAction"/>.</param>
    /// <param name="unitOfWork">Unit of work for committing the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated count of suggested actions on the signal.</returns>
    public static async Task<string> Handle(
        SuggestAction request,
        ISignalRepository signalRepository,
        ISignalActionRepository signalActionRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var signal = await signalRepository.GetByIdOrThrowAsync(request.SignalId, cancellationToken).ConfigureAwait(false);

        SuggestedAction action = request.Parameters switch
        {
            MakeACallParameters p => SuggestedAction.SuggestMakeACall(request.SignalId, p.PhoneNumber, p.Details, request.SuggestedBy),
            VisitAWebpageParameters p => SuggestedAction.SuggestVisitAWebsite(request.SignalId, p.Url, p.Details, request.SuggestedBy),
            _ => throw new InvalidOperationException($"Unable to create suggested action of type {request.Parameters.GetType()}")
        };

        signalActionRepository.Stage(action);
        await unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        return action.Id.Value;
    }
}
