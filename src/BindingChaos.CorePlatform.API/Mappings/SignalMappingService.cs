using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SignalAwareness.Application.ReadModels;
using BindingChaos.SignalAwareness.Domain.Signals;

namespace BindingChaos.CorePlatform.API.Mappings;

/// <summary>Pure structural mapping from signal read models to API response contracts.</summary>
internal static class SignalMapper
{
    /// <summary>Maps a <see cref="SignalView"/> to a <see cref="SignalResponse"/> using pre-resolved pseudonyms.</summary>
    /// <param name="signal">The signal read model to map.</param>
    /// <param name="currentParticipantId">The current participant, used to determine originator and amplification status.</param>
    /// <param name="pseudonyms">Pre-resolved pseudonym map (userId → pseudonym) scoped to the signal.</param>
    /// <returns>The mapped <see cref="SignalResponse"/>.</returns>
    internal static SignalResponse ToSignalResponse(
        SignalView signal,
        ParticipantId currentParticipantId,
        IReadOnlyDictionary<string, string> pseudonyms)
    {
        var amplifications = signal.Amplifications.Select(amp => new SignalResponse.Amplification(
            amp.Id,
            pseudonyms[amp.AmplifierId],
            AmplificationReason.FromValue(amp.Reason).DisplayName,
            amp.Commentary,
            amp.AmplifiedAt)).ToList();

        var attachments = signal.Attachments.Select(att => new AttachmentResponse(
            att.Id,
            att.DocumentId,
            att.Caption,
            att.CreatedAt)).ToArray();

        var suggestedActions = signal.SuggestedActions.Select(a => new SignalResponse.SuggestedAction(
            a.Id,
            a.ActionTypeName,
            a.PhoneNumber,
            a.Url,
            a.Details,
            pseudonyms[a.SuggestedById],
            a.SuggestedAt)).ToList();

        return new SignalResponse(
            signal.Id,
            signal.Title,
            signal.Description,
            pseudonyms[signal.OriginatorId],
            SignalStatus.FromValue(signal.Status).DisplayName,
            signal.CapturedAt,
            signal.Latitude,
            signal.Longitude,
            amplifications,
            signal.Amplifications.Any(a => a.AmplifierId == currentParticipantId.Value),
            signal.OriginatorId == currentParticipantId.Value,
            attachments,
            suggestedActions);
    }

    /// <summary>Maps a <see cref="SignalsListItemView"/> to a <see cref="SignalListItemResponse"/>.</summary>
    /// <param name="signal">The list-item read model to map.</param>
    /// <param name="currentParticipantId">The current participant, used to determine originator and amplification status.</param>
    /// <param name="originatorPseudonym">The pre-resolved pseudonym for the signal originator.</param>
    /// <returns>The mapped <see cref="SignalListItemResponse"/>.</returns>
    internal static SignalListItemResponse ToSignalListItemResponse(
        SignalsListItemView signal,
        ParticipantId currentParticipantId,
        string originatorPseudonym)
    {
        var attachments = signal.Attachments.Select(att => new AttachmentResponse(
            att.Id,
            att.DocumentId,
            att.Caption,
            att.CreatedAt)).ToArray();

        return new SignalListItemResponse(
            signal.Id,
            signal.Title,
            signal.Description,
            originatorPseudonym,
            SignalStatus.FromValue(signal.Status).DisplayName,
            signal.CapturedAt,
            signal.Latitude,
            signal.Longitude,
            signal.AmplificationCount,
            [.. signal.Tags],
            signal.AmplifierIds.Contains(currentParticipantId.Value),
            signal.OriginatorId == currentParticipantId.Value,
            attachments);
    }

    /// <summary>Maps a <see cref="SignalAmplificationTrendView"/> to a <see cref="SignalAmplificationTrendResponse"/>.</summary>
    /// <param name="trendView">The trend data to map.</param>
    /// <returns>The mapped <see cref="SignalAmplificationTrendResponse"/>.</returns>
    internal static SignalAmplificationTrendResponse ToSignalAmplificationTrendResponse(
        SignalAmplificationTrendView trendView)
    {
        var dataPoints = trendView.DataPoints
            .OrderBy(dp => dp.Date)
            .Select(dp => new TrendPointResponse
            {
                Date = dp.Date.ToString("O", System.Globalization.CultureInfo.InvariantCulture),
                EventType = dp.EventType,
            })
            .ToList();

        return new SignalAmplificationTrendResponse
        {
            SignalId = trendView.SignalId,
            DataPoints = dataPoints,
        };
    }
}
