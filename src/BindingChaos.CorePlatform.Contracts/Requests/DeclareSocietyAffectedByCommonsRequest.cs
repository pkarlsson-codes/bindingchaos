namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>Request to declare a society as affected by a commons.</summary>
/// <param name="CommonsId">The ID of the commons this society is affected by.</param>
public record DeclareSocietyAffectedByCommonsRequest(string CommonsId);
