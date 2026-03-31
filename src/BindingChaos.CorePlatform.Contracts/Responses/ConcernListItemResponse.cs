namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Represents a concern item in a list response.
/// </summary>
/// <param name="Id">The unique identifier of the concern.</param>
/// <param name="RaisedByPseudonym">The pseudonym of the user who raised the concern.</param>
/// <param name="Name">The name of the concern.</param>
/// <param name="Tags">The tags associated with the concern.</param>
/// <param name="Signals">The signals related to the concern.</param>
public sealed record ConcernListItemResponse(
    string Id,
    string RaisedByPseudonym,
    string Name,
    IReadOnlyList<string> Tags,
    IReadOnlyList<ConcernListItemResponse.ReferenceSignal> Signals)
{
    /// <summary>
    /// Represents a signal referenced by a concern, including its unique identifier and title.
    /// </summary>
    /// <param name="Id">The unique identifier of the signal.</param>
    /// <param name="Title">The title of the signal.</param>
    public sealed record ReferenceSignal(string Id, string Title);
}