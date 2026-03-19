using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.SuggestedActions;

/// <summary>
/// Represents the category of a suggested action on a signal.
/// </summary>
public sealed class ActionType : Enumeration<ActionType>
{
    /// <summary>
    /// Suggests calling a phone number related to the signal.
    /// </summary>
    public static readonly ActionType MakeACall = new(1, nameof(MakeACall));

    /// <summary>
    /// Suggests visiting a relevant webpage.
    /// </summary>
    public static readonly ActionType VisitAWebpage = new(2, nameof(VisitAWebpage));

    private ActionType(int id, string name)
        : base(id, name)
    {
    }
}
