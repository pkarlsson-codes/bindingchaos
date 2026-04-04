namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Represents a user group item in a list response.
/// </summary>
/// <param name="Id">The unique identifier of the user group.</param>
/// <param name="CommonsId">The identifier of the commons this group governs.</param>
/// <param name="Name">The name of the user group.</param>
/// <param name="Philosophy">The philosophy of the user group.</param>
/// <param name="FoundedByPseudonym">The pseudonym of the participant who founded the group.</param>
/// <param name="FormedAt">The timestamp when the group was formed.</param>
/// <param name="MemberCount">The current number of members in the group.</param>
/// <param name="JoinPolicy">The join policy of the group.</param>
public sealed record UserGroupListItemResponse(
    string Id,
    string CommonsId,
    string Name,
    string Philosophy,
    string FoundedByPseudonym,
    DateTimeOffset FormedAt,
    int MemberCount,
    string JoinPolicy);
