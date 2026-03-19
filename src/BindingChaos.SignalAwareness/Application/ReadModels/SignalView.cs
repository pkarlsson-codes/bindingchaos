namespace BindingChaos.SignalAwareness.Application.ReadModels;

/// <summary>
/// Read model entity for signals optimized for querying.
/// This is populated from EventStoreDB events via projections.
/// </summary>
public class SignalView
{
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
    /// The version number for optimistic concurrency control.
    /// </summary>
    required public long Version { get; set; }

    /// <summary>
    /// Cached count of amplifications to support efficient sorting/filtering.
    /// </summary>
    public int AmplificationCount { get; set; }

    /// <summary>
    /// Collection of tags associated with the signal.
    /// </summary>
    required public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Collection of amplifications for this signal.
    /// </summary>
    required public List<SignalAmplificationView> Amplifications { get; set; }

    /// <summary>
    /// Gets or sets the collection of attachments associated with the entity.
    /// </summary>
    public List<Attachment> Attachments { get; set; } = [];

    /// <summary>
    /// Collection of suggested actions for this signal.
    /// </summary>
    public List<SuggestedActionView> SuggestedActions { get; set; } = [];

    /// <summary>
    /// When the signal was last updated.
    /// </summary>
    public DateTimeOffset LastUpdatedAt { get; internal set; }

    /// <summary>
    /// Represents an attachment associated with a signal.
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// The unique identifier for the attachment.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the caption text associated with the object.
        /// </summary>
        public string? Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the document.
        /// </summary>
        public string DocumentId { get; set; } = string.Empty;
    }
}
