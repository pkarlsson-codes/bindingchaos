namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request used to raise a new concern in the system.
/// </summary>
/// <param name="Name">Name of the concern being raised.</param>
/// <param name="Tags">Tags associated with the raised concern.</param>
/// <param name="SignalIds">Ids of signals associated with the concern.</param>
public record RaiseConcernRequest(
    string Name,
    IReadOnlyList<string> Tags,
    IReadOnlyList<string> SignalIds);