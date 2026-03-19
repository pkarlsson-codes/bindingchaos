using System.Reflection;

namespace BindingChaos.SignalAwareness.Infrastructure;

/// <summary>
/// Marker surface for assembly registration for SignalAwareness.
/// </summary>
public static class SignalAwarenessAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains handlers for the SignalAwareness bounded context.
    /// </summary>
    public static Assembly Assembly => typeof(SignalAwarenessAssemblyReference).Assembly;
}
