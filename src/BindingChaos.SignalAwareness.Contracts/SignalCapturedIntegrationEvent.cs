using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.SignalAwareness.Contracts;

/// <summary>
/// Integration event published when a signal is captured in the SignalAwareness bounded context.
/// This event is used to notify other bounded contexts about signal capture without creating direct dependencies.
/// </summary>
/// <param name="SignalId">The unique identifier of the captured signal.</param>
/// <param name="Title">The title of the signal.</param>
/// <param name="Description">The description of the signal.</param>
/// <param name="OriginatorId">The identifier of the participant who captured the signal.</param>
/// <param name="Latitude">The optional latitude of the location where this signal occurred.</param>
/// <param name="Longitude">The optional longitude of the location where this signal occurred.</param>
/// <param name="Tags">Optional tags associated with the signal.</param>
/// <param name="CapturedAt">When the signal was captured.</param>
public sealed record SignalCapturedIntegrationEvent(
    string SignalId,
    string Title,
    string Description,
    string OriginatorId,
    double? Latitude,
    double? Longitude,
    string[] Tags,
    DateTimeOffset CapturedAt
) : IntegrationEvent;
