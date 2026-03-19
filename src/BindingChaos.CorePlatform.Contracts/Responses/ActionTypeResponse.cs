namespace BindingChaos.CorePlatform.Contracts.Responses;

/// <summary>
/// Response model representing an available action type in the catalog.
/// </summary>
/// <param name="Id">The stable integer identifier used in events and API calls.</param>
/// <param name="Name">
/// The code name of the action type (e.g. <c>MakeACall</c>).
/// Use this value as <c>ActionType</c> when submitting a suggested action.
/// Display labels for this name are the responsibility of the presentation layer.
/// </param>
public record ActionTypeResponse(int Id, string Name);
