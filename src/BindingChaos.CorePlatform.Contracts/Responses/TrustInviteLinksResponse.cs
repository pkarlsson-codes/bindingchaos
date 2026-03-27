namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// The list of invite links returned from the API.
/// </summary>
/// <param name="Items">All invite links (active and revoked) for the participant, sorted by creation date descending.</param>
public sealed record TrustInviteLinksResponse(IReadOnlyList<TrustInviteLinkViewResponse> Items);
