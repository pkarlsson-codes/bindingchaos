using BindingChaos.SharedKernel.Domain.Events;

namespace BindingChaos.Stigmergy.Contracts;

/// <summary>
/// Signal captured integration event.
/// </summary>
/// <param name="SignalId">Id of the captured signal.</param>
/// <param name="CapturedById">Id of actor that captured the signal.</param>
/// <param name="Description">Description of the captured signal.</param>
/// <param name="Tags">Tags assigned to the captured signal.</param>
public sealed record SignalCapturedIntegrationEvent(
    string SignalId,
    string CapturedById,
    string Description,
    IReadOnlyList<string> Tags
) : IntegrationEvent;