using System.Reflection;

namespace BindingChaos.Tagging.Infrastructure;

/// <summary>
/// Marker surface for MediatR assembly registration for Tagging.
/// </summary>
public static class TaggingAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains the <see cref="TaggingAssemblyReference"/> type.
    /// </summary>
    public static Assembly Assembly => typeof(TaggingAssemblyReference).Assembly;
}
