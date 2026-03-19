using System.Reflection;
using BindingChaos.CommunityDiscourse.Application.Commands;

namespace BindingChaos.CommunityDiscourse.Infrastructure;

/// <summary>
/// Marker surface for MediatR assembly registration for the CommunityDiscourse bounded context.
/// </summary>
public class CommunityDiscourseAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains MediatR handlers for the SignalAwareness bounded context.
    /// </summary>
    public static Assembly Assembly => typeof(PostContribution).Assembly;
}
