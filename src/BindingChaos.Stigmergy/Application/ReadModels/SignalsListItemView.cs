using System.Linq.Expressions;

namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// A lightweight read model for signal list views.
/// </summary>
public class SignalsListItemView
{
    /// <summary>
    /// Sort mappings for use with <see cref="Infrastructure.Querying.IQueryableExtensions.Sort"/>.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Expression<Func<SignalsListItemView, object>>> SortMappings =
        new Dictionary<string, Expression<Func<SignalsListItemView, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["capturedAt"] = s => s.CapturedAt,
            ["createdAt"] = s => s.CapturedAt,
            ["amplificationCount"] = s => s.AmplificationCount,
            ["lastAmplifiedAt"] = s => s.LastAmplifiedAt!,
            ["title"] = s => s.Title,
        };

    /// <summary>Gets or sets the signal ID.</summary>
    public required string Id { get; set; }

    /// <summary>Gets or sets the signal title.</summary>
    public required string Title { get; set; }

    /// <summary>Gets or sets the signal description.</summary>
    public required string Description { get; set; }

    /// <summary>Gets or sets the ID of the participant who captured the signal.</summary>
    public required string CapturedById { get; set; }

    /// <summary>Gets or sets when the signal was captured.</summary>
    public required DateTimeOffset CapturedAt { get; set; }

    /// <summary>Gets or sets the tags associated with the signal.</summary>
    public List<string> Tags { get; set; } = [];

    /// <summary>Gets or sets the attachment document IDs.</summary>
    public List<string> AttachmentIds { get; set; } = [];

    /// <summary>Gets or sets the number of active amplifications.</summary>
    public int AmplificationCount { get; set; }

    /// <summary>Gets or sets the IDs of participants who have amplified this signal.</summary>
    public List<string> AmplifierIds { get; set; } = [];

    /// <summary>Gets or sets when the signal was last amplified.</summary>
    public DateTimeOffset? LastAmplifiedAt { get; set; }

    /// <summary>Gets or sets the latitude of where the signal was captured, if provided.</summary>
    public double? Latitude { get; set; }

    /// <summary>Gets or sets the longitude of where the signal was captured, if provided.</summary>
    public double? Longitude { get; set; }
}
