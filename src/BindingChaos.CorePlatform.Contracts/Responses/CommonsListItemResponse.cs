namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Represents a commons item in a list response.
/// </summary>
/// <param name="Id">The unique identifier of the commons.</param>
/// <param name="Name">The name of the commons.</param>
/// <param name="Description">The description of the commons.</param>
/// <param name="Status">The lifecycle status of the commons.</param>
/// <param name="ProposedByPseudonym">The pseudonym of the participant who proposed the commons.</param>
/// <param name="ProposedAt">The timestamp when the commons was proposed.</param>
public sealed record CommonsListItemResponse(
    string Id,
    string Name,
    string Description,
    string Status,
    string ProposedByPseudonym,
    DateTimeOffset ProposedAt);
