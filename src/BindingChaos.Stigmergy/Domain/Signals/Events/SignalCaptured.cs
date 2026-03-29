using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Domain.Signals.Events;

/// <summary>
/// Signal captured event.
/// </summary>
/// <param name="AggregateId">Id of the captured signal.</param>
/// <param name="CapturedById">Id of the actor capturing the signal.</param>
/// <param name="Title">Title of the signal.</param>
/// <param name="Description">Text description of the signal.</param>
/// <param name="Tags">Tags attached to the signal.</param>
/// <param name="AttachmentIds">Ids of documents attached to the signal.</param>
/// <param name="Latitude">Latitude of where the signal was captured.</param>
/// <param name="Longitude">Longitude of where the signal was captured.</param>
public record SignalCaptured(
    string AggregateId,
    string CapturedById,
    string Title,
    string Description,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> AttachmentIds,
    double? Latitude,
    double? Longitude
) : DomainEvent(AggregateId);