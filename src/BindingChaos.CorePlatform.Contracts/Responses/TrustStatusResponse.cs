namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model indicating whether the authenticated participant directly trusts a given participant.
/// </summary>
/// <param name="Trusted"><see langword="true"/> if a direct trust relationship exists; otherwise <see langword="false"/>.</param>
public sealed record TrustStatusResponse(bool Trusted);
