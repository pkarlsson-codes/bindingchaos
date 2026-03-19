namespace BindingChaos.CorePlatform.Contracts.Responses.Tags;

/// <summary>
/// A tag response.
/// </summary>
/// <param name="TagId">The id of the tag.</param>
/// <param name="Label">The label of the tag.</param>
/// <param name="UsageCount">The number of entities using the tag in its locality.</param>
public sealed record TagResponse(
    string TagId,
    string Label,
    int UsageCount);
