using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SignalAwareness.Domain.Signals;

/// <summary>
/// Represents the current status of a signal.
/// </summary>
public sealed class SignalStatus : Enumeration<SignalStatus>
{
    /// <summary>
    /// Signal is active and visible to participants.
    /// </summary>
    public static readonly SignalStatus Active = new(0, nameof(Active));

    /// <summary>
    /// Signal has expired due to age or inactivity.
    /// </summary>
    public static readonly SignalStatus Expired = new(1, nameof(Expired));

    /// <summary>
    /// Signal has been archived and is no longer visible.
    /// </summary>
    public static readonly SignalStatus Archived = new(2, nameof(Archived));

    /// <summary>
    /// Initializes a new instance of the SignalStatus class.
    /// </summary>
    /// <param name="id">The unique identifier for this status.</param>
    /// <param name="name">The name of this status.</param>
    private SignalStatus(int id, string name)
        : base(id, name)
    {
    }
}