using System.Linq.Expressions;

namespace BindingChaos.SignalAwareness.Application.ReadModels;

/// <summary>
/// Lightweight read model optimized for signal listing and feeds.
/// Contains only essential data for list displays and filtering.
/// </summary>
public class SignalsListItemView
{
    /// <summary>
    /// Logical sort mappings for list queries.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<SignalsListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<SignalsListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["capturedAt"] = s => s.CapturedAt,
            ["createdAt"] = s => s.CapturedAt,
            ["lastUpdatedAt"] = s => s.LastUpdatedAt,
            ["lastAmplifiedAt"] = s => s.LastAmplifiedAt!,
            ["amplificationCount"] = s => s.AmplificationCount,
            ["title"] = s => s.Title,
        };

    /// <summary>
    /// The unique identifier for the signal.
    /// </summary>
    required public string Id { get; set; }

    /// <summary>
    /// The title of the signal.
    /// </summary>
    required public string Title { get; set; }

    /// <summary>
    /// The description of the signal.
    /// </summary>
    required public string Description { get; set; }

    /// <summary>
    /// The identifier of the participant who originated the signal.
    /// </summary>
    required public string OriginatorId { get; set; }

    /// <summary>
    /// When the signal was last amplified (for sorting purposes).
    /// </summary>
    public DateTimeOffset? LastAmplifiedAt { get; set; }

    /// <summary>
    /// When the signal was last updated.
    /// </summary>
    public DateTimeOffset LastUpdatedAt { get; internal set; }

    /// <summary>
    /// The optional latitude of the location where this signal occurred.
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// The optional longitude of the location where this signal occurred.
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// The current status of the signal.
    /// </summary>
    required public int Status { get; set; }

    /// <summary>
    /// When the signal was captured.
    /// </summary>
    required public DateTimeOffset CapturedAt { get; set; }

    /// <summary>
    /// Cached count of amplifications for efficient sorting/filtering.
    /// </summary>
    public int AmplificationCount { get; set; }

    /// <summary>
    /// Collection of tags associated with the signal.
    /// </summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Collection of participant IDs who have amplified this signal.
    /// </summary>
    public List<string> AmplifierIds { get; set; } = [];

    /// <summary>
    /// Collection of attachments associated with this signal for list view display.
    /// </summary>
    public List<AttachmentListItem> Attachments { get; set; } = [];
}

/// <summary>
/// Lightweight attachment information for list views.
/// </summary>
public class AttachmentListItem
{
    /// <summary>
    /// The unique identifier for the attachment.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The document ID stored in the document service.
    /// </summary>
    public string DocumentId { get; set; } = string.Empty;

    /// <summary>
    /// Optional caption for the attachment.
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// When the attachment was added.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
}