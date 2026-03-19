using BindingChaos.SignalAwareness.Application.ReadModels;
using BindingChaos.SignalAwareness.Domain.Signals;
using BindingChaos.SignalAwareness.Domain.Signals.Events;
using BindingChaos.SignalAwareness.Domain.SuggestedActions;
using BindingChaos.SignalAwareness.Domain.SuggestedActions.Events;
using JasperFx.Events;
using Marten.Events.Projections;

namespace BindingChaos.SignalAwareness.Infrastructure.Projections;

/// <summary>
/// Marten projection that builds SignalReadModel from Signal events.
/// Pseudonyms are generated at read-time by the service layer, not during projection.
/// </summary>
internal class SignalViewProjection : MultiStreamProjection<SignalView, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SignalViewProjection"/> class.
    /// </summary>
    public SignalViewProjection()
    {
        Identity<SignalCaptured>(e => e.AggregateId);
        Identity<SignalAmplified>(e => e.AggregateId);
        Identity<SignalAmplificationAttenuated>(e => e.AggregateId);
        Identity<SignalContentUpdated>(e => e.AggregateId);
        Identity<CallActionSuggested>(e => e.SignalId);
        Identity<WebpageActionSuggested>(e => e.SignalId);
    }

    /// <summary>
    /// Applies a <see cref="SignalAmplified"/> event to the read model view.
    /// </summary>
    /// <param name="view">The read model to mutate.</param>
    /// <param name="e">The amplified event.</param>
    public static void Apply(SignalView view, SignalAmplified e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(view);

        view.Amplifications.Add(new SignalAmplificationView
        {
            Id = e.AmplificationId,
            AmplifiedAt = e.OccurredAt,
            AmplifierId = e.AmplifierId,
            Reason = e.Reason,
            Commentary = e.Commentary,
            IsActive = true,
        });

        view.AmplificationCount += 1;
    }

    /// <summary>
    /// Applies a <see cref="SignalAmplificationAttenuated"/> event to the read model view.
    /// </summary>
    /// <param name="view">The read model to mutate.</param>
    /// <param name="e">The attenuated amplification event.</param>
    public static void Apply(SignalView view, SignalAmplificationAttenuated e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(view);

        var amplification = view.Amplifications.FirstOrDefault(a => a.Id == e.AmplificationId);
        if (amplification != null)
        {
            view.Amplifications.Remove(amplification);
            view.AmplificationCount = view.Amplifications.Count;
        }
    }

    /// <summary>
    /// Applies a <see cref="SignalContentUpdated"/> event to the read model view.
    /// </summary>
    /// <param name="view">The read model to mutate.</param>
    /// <param name="e">The updated content event.</param>
    public static void Apply(SignalView view, SignalContentUpdated e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(view);

        view.Description = e.Description;
        view.Title = e.Title;
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Applies a <see cref="SignalStatusChanged"/> event to the read model view.
    /// </summary>
    /// <param name="view">The read model to mutate.</param>
    /// <param name="e">The status change event.</param>
    public static void Apply(SignalView view, SignalStatusChanged e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(view);

        view.Status = e.NewStatus;
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Applies the specified attachment addition event to the given signal view.
    /// </summary>
    /// <param name="view">The signal view to which the attachment will be added. Cannot be <see langword="null"/>.</param>
    /// <param name="e">The event containing the details of the attachment to add. Cannot be <see langword="null"/>.</param>
    public static void Apply(SignalView view, AttachmentAdded e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(view);
        view.Attachments.Add(new SignalView.Attachment
        {
            Id = e.AttachmentId,
            DocumentId = e.DocumentId,
            Caption = e.Caption,
            CreatedAt = e.OccurredAt,
        });
        view.LastUpdatedAt = e.OccurredAt;
    }

    /// <summary>
    /// Applies a <see cref="CallActionSuggested"/> event to the read model view.
    /// </summary>
    /// <param name="view">The read model to mutate.</param>
    /// <param name="e">The suggested call action event.</param>
    public static void Apply(SignalView view, CallActionSuggested e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(view);

        view.SuggestedActions.Add(new SuggestedActionView
        {
            Id = e.AggregateId,
            ActionTypeId = ActionType.MakeACall.Value,
            ActionTypeName = ActionType.MakeACall.DisplayName,
            PhoneNumber = e.PhoneNumber,
            Details = e.Details,
            SuggestedById = e.SuggestedBy,
            SuggestedAt = e.OccurredAt,
        });
    }

    /// <summary>
    /// Applies a <see cref="WebpageActionSuggested"/> event to the read model view.
    /// </summary>
    /// <param name="view">The read model to mutate.</param>
    /// <param name="e">The suggested webpage action event.</param>
    public static void Apply(SignalView view, WebpageActionSuggested e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(view);

        view.SuggestedActions.Add(new SuggestedActionView
        {
            Id = e.AggregateId,
            ActionTypeId = ActionType.VisitAWebpage.Value,
            ActionTypeName = ActionType.VisitAWebpage.DisplayName,
            Url = e.Url,
            Details = e.Details,
            SuggestedById = e.SuggestedBy,
            SuggestedAt = e.OccurredAt,
        });
    }

    /// <summary>
    /// Creates the initial read model view from a <see cref="SignalCaptured"/> event.
    /// </summary>
    /// <param name="e">The captured event wrapped by Marten.</param>
    /// <returns>The initialized signal view.</returns>
    public static SignalView Create(IEvent<SignalCaptured> e)
    {
        ArgumentNullException.ThrowIfNull(e);

        return new SignalView
        {
            Amplifications = [],
            CapturedAt = e.Data.OccurredAt,
            Description = e.Data.Description,
            Id = e.Data.AggregateId,
            Latitude = e.Data.Latitude,
            Longitude = e.Data.Longitude,
            Title = e.Data.Title,
            OriginatorId = e.Data.OriginatorId,
            Status = SignalStatus.Active.Value,
            Tags = e.Data.Tags == null ? [] : [.. e.Data.Tags],
            Version = e.Version,
            AmplificationCount = 0,
            LastUpdatedAt = e.Data.OccurredAt,
            Attachments =
            [
                .. e.Data.Attachments.Select(a => new SignalView.Attachment
                {
                    Id = a.AttachmentId,
                    DocumentId = a.DocumentId,
                    Caption = a.Caption,
                    CreatedAt = e.Data.OccurredAt,
                })
            ],
        };
    }
}
