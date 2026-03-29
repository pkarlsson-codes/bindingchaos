using BindingChaos.SharedKernel.Domain.Events;
using Microsoft.Extensions.Diagnostics.Latency;

namespace BindingChaos.Stigmergy.Domain.Signals.Events;

/// <summary>
/// Signal captured event.
/// </summary>
/// <param name="AggregateId">Id of the captured signal.</param>
/// <param name="CapturedById">Id of the actor capturing the signal.</param>
/// <param name="Description">Text description of the signal.</param>
/// <param name="Tags">Tags attached to the signal.</param>
public record SignalCaptured(
    string AggregateId,
    string CapturedById,
    string Description,
    IReadOnlyList<string> Tags
) : DomainEvent(AggregateId);