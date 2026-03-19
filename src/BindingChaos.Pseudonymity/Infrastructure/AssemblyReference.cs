using System.Reflection;

namespace BindingChaos.Pseudonymity.Infrastructure;

/// <summary>
/// Marker surface for MediatR assembly registration for Pseudonymity.
/// </summary>
public static class PseudonymityAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains MediatR handlers for the Pseudonymity bounded context.
    /// </summary>
    public static Assembly Assembly => typeof(PseudonymityAssemblyReference).Assembly;
}


