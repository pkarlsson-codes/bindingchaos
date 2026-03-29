using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Events;
using BindingChaos.SharedKernel.Domain.Geography;
using BindingChaos.Stigmergy.Domain.Signals.Events;

namespace BindingChaos.Stigmergy.Domain.Signals;

/// <summary>
/// A Signal aggregate.
/// </summary>
public sealed class Signal : AggregateRoot<SignalId>
{
    private Signal() { }

    /// <summary>
    /// Capture a signal.
    /// </summary>
    /// <param name="actorId">Id of the actor capturing the signal.</param>
    /// <param name="title">Title of the signal.</param>
    /// <param name="description">Description of the signal.</param>
    /// <param name="tags">Tags for the signal.</param>
    /// <param name="attachmentIds">Ids of documents attached to the signal.</param>
    /// <param name="coordinates">Optional coordinates of where the signal was captured.</param>
    /// <returns>The captured signal.</returns>
    public static Signal Capture(
        ParticipantId actorId,
        string title,
        string description,
        IReadOnlyList<string> tags,
        IReadOnlyList<string> attachmentIds,
        Coordinates? coordinates)
    {
        ArgumentNullException.ThrowIfNull(actorId);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        var signal = new Signal();
        var aggregateId = SignalId.Generate();
        signal.ApplyChange(
            new SignalCaptured(
                aggregateId.Value,
                actorId.Value,
                title,
                description,
                tags,
                attachmentIds,
                coordinates?.Latitude,
                coordinates?.Longitude));
        return signal;
    }

    /// <inheritdoc/>
    protected override void ApplyEvent(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case SignalCaptured e: Apply(e); break;
            default: throw new InvalidOperationException($"Unknown event type: {domainEvent.GetType().Name}");
        }
    }

    private void Apply(SignalCaptured e)
    {
        Id = SignalId.Create(e.AggregateId);
    }
}