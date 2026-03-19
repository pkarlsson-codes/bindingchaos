namespace BindingChaos.CorePlatform.Contracts.Requests;

/// <summary>
/// Request model for suggesting a structured action on a signal.
/// </summary>
/// <param name="ActionType">
/// The action type name (e.g. <c>MakeACall</c>, <c>VisitAWebpage</c>).
/// Must match the <c>Name</c> of a value returned by <c>GET /api/action-types</c>.
/// </param>
/// <param name="PhoneNumber">The phone number to call. Required when <paramref name="ActionType"/> is <c>MakeACall</c>.</param>
/// <param name="Url">The URL to visit. Required when <paramref name="ActionType"/> is <c>VisitAWebpage</c>.</param>
/// <param name="Details">Optional free-text context applicable to any action type.</param>
public record SuggestActionRequest(
    string? ActionType,
    string? PhoneNumber,
    string? Url,
    string? Details);
