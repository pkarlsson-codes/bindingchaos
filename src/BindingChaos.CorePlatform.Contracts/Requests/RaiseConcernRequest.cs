using System.Text.Json.Serialization;

namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Describes how a concern came to be raised.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ConcernOriginDto
{
    /// <summary>A participant manually identified a pattern and raised the concern directly.</summary>
    Manual,

    /// <summary>A signal cluster was detected and a participant confirmed it as a concern.</summary>
    EmergingPattern,
}

/// <summary>
/// Request used to raise a new concern in the system.
/// </summary>
/// <param name="Name">Name of the concern being raised.</param>
/// <param name="Tags">Tags associated with the raised concern.</param>
/// <param name="SignalIds">Ids of signals associated with the concern.</param>
/// <param name="Origin">How the concern came to be raised.</param>
/// <param name="ClusterId">Id of the signal cluster, when origin is <see cref="ConcernOriginDto.EmergingPattern"/>.</param>
public record RaiseConcernRequest(
    string Name,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> SignalIds,
    ConcernOriginDto Origin,
    string? ClusterId = null);