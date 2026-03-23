using System.Reflection;

namespace BindingChaos.Reputation.Infrastructure;

/// <summary>
/// Marker surface for assembly registration for Reputation.
/// </summary>
public static class ReputationAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains the <see cref="ReputationAssemblyReference"/> type.
    /// </summary>
    public static Assembly Assembly => typeof(ReputationAssemblyReference).Assembly;
}
