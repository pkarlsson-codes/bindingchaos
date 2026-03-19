using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.SuggestedActions;

/// <summary>
/// Base value object for the structured parameters of a suggested action.
/// Each subtype corresponds to exactly one <see cref="ActionType"/> and carries
/// only the fields relevant to that type, making mismatches impossible.
/// </summary>
public abstract class ActionParameters : ValueObject
{
    /// <summary>
    /// The action type these parameters belong to.
    /// Derived from the concrete subtype — no separate argument needed.
    /// </summary>
    public abstract ActionType ActionType { get; }
}

/// <summary>
/// Parameters for a <see cref="ActionType.MakeACall"/> suggested action.
/// </summary>
/// <param name="phoneNumber">The phone number to call.</param>
/// <param name="details">Optional context about the call (e.g. what to ask about).</param>
public sealed class MakeACallParameters(string phoneNumber, string? details = null) : ActionParameters
{
    /// <summary>Gets the phone number to call.</summary>
    public string PhoneNumber { get; } = phoneNumber;

    /// <summary>Gets optional context about the call.</summary>
    public string? Details { get; } = details;

    /// <inheritdoc />
    public override ActionType ActionType => ActionType.MakeACall;

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ActionType.Value;
        yield return PhoneNumber;
        yield return Details ?? string.Empty;
    }
}

/// <summary>
/// Parameters for a <see cref="ActionType.VisitAWebpage"/> suggested action.
/// </summary>
/// <param name="url">The URL to visit.</param>
/// <param name="details">Optional context about what to do on the page.</param>
public sealed class VisitAWebpageParameters(string url, string? details = null) : ActionParameters
{
    /// <summary>Gets the URL to visit.</summary>
    public string Url { get; } = url;

    /// <summary>Gets optional context about what to do on the page.</summary>
    public string? Details { get; } = details;

    /// <inheritdoc />
    public override ActionType ActionType => ActionType.VisitAWebpage;

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ActionType.Value;
        yield return Url;
        yield return Details ?? string.Empty;
    }
}
