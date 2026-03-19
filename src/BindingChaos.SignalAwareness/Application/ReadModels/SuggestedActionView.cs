namespace BindingChaos.SignalAwareness.Application.ReadModels;

/// <summary>
/// Read model for a suggested action on a signal, optimized for querying.
/// Populated from domain events via projections.
/// </summary>
public class SuggestedActionView
{
    /// <summary>The unique identifier for the suggested action.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>The integer value of the <see cref="Domain.SuggestedActions.ActionType"/> enumeration.</summary>
    public int ActionTypeId { get; set; }

    /// <summary>The display name of the action type, denormalized at projection time.</summary>
    public string ActionTypeName { get; set; } = string.Empty;

    /// <summary>The phone number to call. Populated for <c>MakeACall</c> actions.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>The URL to visit. Populated for <c>VisitAWebpage</c> actions.</summary>
    public string? Url { get; set; }

    /// <summary>Optional free-text context provided by the suggester.</summary>
    public string? Details { get; set; }

    /// <summary>The identifier of the participant who suggested the action.</summary>
    public string SuggestedById { get; set; } = string.Empty;

    /// <summary>When the action was suggested.</summary>
    public DateTimeOffset SuggestedAt { get; set; }
}
