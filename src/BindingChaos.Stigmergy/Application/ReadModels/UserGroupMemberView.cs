namespace BindingChaos.Stigmergy.Application.ReadModels;

/// <summary>
/// Read model representing a resolved member of a user group.
/// </summary>
/// <param name="Pseudonym">The pseudonym of the member.</param>
public sealed record UserGroupMemberView(string Pseudonym);
