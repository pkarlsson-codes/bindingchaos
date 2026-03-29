namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Read model for a single signal's full detail.
/// </summary>
public class SignalView
{
    /// <summary>Gets or sets the signal ID.</summary>
    required public string Id { get; set; }

    /// <summary>Gets or sets the signal title.</summary>
    required public string Title { get; set; }

    /// <summary>Gets or sets the signal description.</summary>
    required public string Description { get; set; }

    /// <summary>Gets or sets the ID of the participant who captured the signal.</summary>
    required public string CapturedById { get; set; }

    /// <summary>Gets or sets when the signal was captured.</summary>
    required public DateTimeOffset CapturedAt { get; set; }

    /// <summary>Gets or sets the tags associated with the signal.</summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>Gets or sets the attachment document IDs.</summary>
    public List<string> AttachmentIds { get; set; } = [];

    /// <summary>Gets or sets the number of active amplifications.</summary>
    public int AmplificationCount { get; set; }

    /// <summary>Gets or sets the active amplifications for this signal.</summary>
    public List<AmplificationEntry> Amplifications { get; set; } = [];

    /// <summary>Gets or sets when the signal was last amplified.</summary>
    public DateTimeOffset? LastAmplifiedAt { get; set; }

    /// <summary>Gets or sets the latitude of where the signal was captured, if provided.</summary>
    public double? Latitude { get; set; }

    /// <summary>Gets or sets the longitude of where the signal was captured, if provided.</summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// An individual amplification of a signal.
    /// </summary>
    public class AmplificationEntry
    {
        /// <summary>Gets or sets the ID of the participant who amplified the signal.</summary>
        required public string AmplifiedById { get; set; }

        /// <summary>Gets or sets when the amplification occurred.</summary>
        required public DateTimeOffset AmplifiedAt { get; set; }
    }
}
