using System.Reflection;

namespace BindingChaos.Societies.Infrastructure;

/// <summary>
/// Marker surface for Wolverine assembly registration for Societies.
/// </summary>
public static class SocietiesAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains Wolverine handlers for the Societies bounded context.
    /// </summary>
    public static Assembly Assembly => typeof(SocietiesAssemblyReference).Assembly;
}
