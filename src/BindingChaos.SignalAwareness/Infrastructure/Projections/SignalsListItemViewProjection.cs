using BindingChaos.SignalAwareness.Application.ReadModels;
using BindingChaos.SignalAwareness.Domain.Signals;
using BindingChaos.SignalAwareness.Domain.Signals.Events;
using JasperFx.Events;
using Marten.Events.Aggregation;

namespace BindingChaos.SignalAwareness.Infrastructure.Projections;

/// <summary>
/// Optimized projection for signal list operations.
/// Avoids loading amplification details.
/// </summary>
internal class SignalsListItemViewProjection : SingleStreamProjection<SignalsListItemView, string>
{
    /// <summary>
    /// Creates a new instance of <see cref="SignalsListItemViewProjection"/>.
    /// </summary>
    /// /// <param name="e">The event containing the signal data.</param>
    /// /// <returns>A new instance of <see cref="SignalsListItemView"/>.</returns>
    public static SignalsListItemView Create(IEvent<SignalCaptured> e)
    {
        return new SignalsListItemView
        {
            Id = e.Data.AggregateId,
            Title = e.Data.Title,
            Description = e.Data.Description,
            OriginatorId = e.Data.OriginatorId,
            Latitude = e.Data.Latitude,
            Longitude = e.Data.Longitude,
            Status = SignalStatus.Active.Value,
            CapturedAt = e.Data.OccurredAt,
            Tags = e.Data.Tags == null ? [] : [..e.Data.Tags],
            AmplificationCount = 0,
            LastAmplifiedAt = null,
            LastUpdatedAt = e.Data.OccurredAt,
        };
    }

    /// <summary>
    /// Applies the captured signal event to the view model.
    /// </summary>
    /// <param name="view">The signal list view to update.</param>
    /// <param name="e">The event containing the signal data.</param>
    public static void Apply(SignalsListItemView view, SignalAmplified e)
    {
        view.AmplificationCount += 1;
        view.LastAmplifiedAt = e.OccurredAt;
        view.AmplifierIds.Add(e.AmplifierId);
    }

    /// <summary>
    /// Applies the signal content updated event to the view model.
    /// </summary>
    /// <param name="view">The signal list view to update.</param>
    /// <param name="e">The event containing the updated signal content.</param>
    public static void Apply(SignalsListItemView view, SignalContentUpdated e)
    {
        view.Title = e.Title;
        view.Description = e.Description;
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Applies the signal status changed event to the view model.
    /// </summary>
    /// <param name="view">The signal list view to update.</param>
    /// <param name="e">The event containing the new signal status.</param>
    public static void Apply(SignalsListItemView view, SignalStatusChanged e)
    {
        view.Status = e.NewStatus;
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Applies the signal amplification attenuated event to the view model.
    /// </summary>
    /// <param name="view">The signal list view to update.</param>
    /// <param name="e">The event containing the attenuated amplification data.</param>
    public static void Apply(SignalsListItemView view, SignalAmplificationAttenuated e)
    {
        view.AmplificationCount = Math.Max(0, view.AmplificationCount - 1);
        view.AmplifierIds.Remove(e.AmplifierId);
    }

    /// <summary>
    /// Applies the attachment added event to the view model.
    /// </summary>
    /// <param name="view">The signal list view to update.</param>
    /// <param name="e">The event containing the attachment data.</param>
    public static void Apply(SignalsListItemView view, AttachmentAdded e)
    {
        var attachment = new AttachmentListItem
        {
            Id = e.AttachmentId,
            DocumentId = e.DocumentId,
            Caption = e.Caption,
            CreatedAt = e.OccurredAt,
        };

        view.Attachments.Add(attachment);
        view.LastUpdatedAt = e.OccurredAt;
    }
}