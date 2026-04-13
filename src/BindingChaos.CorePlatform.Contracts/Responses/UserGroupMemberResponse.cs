namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Represents a single member of a user group.
/// </summary>
/// <param name="Pseudonym">The pseudonym of the member.</param>
public sealed record UserGroupMemberResponse(string Pseudonym);
