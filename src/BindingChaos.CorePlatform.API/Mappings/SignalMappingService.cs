using BindingChaos.CorePlatform.Contracts.Responses;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.Stigmergy.Application.ReadModels;

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
            pseudonyms.GetValueOrDefault(amp.AmplifiedById, "Unknown"),
            amp.AmplifiedAt)).ToList();

        return new SignalResponse(
            signal.Id,
            signal.Title,
            signal.Description,
            pseudonyms.GetValueOrDefault(signal.CapturedById, signal.CapturedById),
            signal.CapturedAt,
            signal.LastAmplifiedAt,
            signal.Latitude,
            signal.Longitude,
            amplifications,
            signal.Amplifications.Any(a => a.AmplifiedById == currentParticipantId.Value),
            signal.CapturedById == currentParticipantId.Value,
            [.. signal.AttachmentIds]);
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
        return new SignalListItemResponse(
            signal.Id,
            signal.Title,
            signal.Description,
            originatorPseudonym,
            signal.CapturedAt,
            signal.Latitude,
            signal.Longitude,
            signal.AmplificationCount,
            [.. signal.Tags],
            signal.AmplifierIds.Contains(currentParticipantId.Value),
            signal.CapturedById == currentParticipantId.Value,
            [.. signal.AttachmentIds]);
    }
}
