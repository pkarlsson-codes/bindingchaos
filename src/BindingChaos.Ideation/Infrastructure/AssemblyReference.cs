using System.Reflection;

namespace BindingChaos.Ideation.Infrastructure;

/// <summary>
/// Marker surface for MediatR assembly registration for Ideation.
/// </summary>
public static class IdeationAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains MediatR handlers for the Ideation bounded context.
    /// </summary>
    public static Assembly Assembly => typeof(IdeationAssemblyReference).Assembly;
}
