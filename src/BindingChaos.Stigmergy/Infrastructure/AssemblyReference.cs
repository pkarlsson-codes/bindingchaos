using System.Reflection;

namespace BindingChaos.Stigmergy.Infrastructure;

/// <summary>
/// Marker surface for Wolverine assembly discovery for Stigmergy.
/// </summary>
public static class StigmergyAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains Wolverine handlers for the Stigmergy bounded context.
    /// </summary>
    public static Assembly Assembly => typeof(StigmergyAssemblyReference).Assembly;
}
