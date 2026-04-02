using System.Reflection;
using BindingChaos.IdentityProfile.Application.Commands;

namespace BindingChaos.IdentityProfile.Infrastructure;

/// <summary>
/// Marker for Wolverine assembly discovery in the IdentityProfile bounded context.
/// </summary>
public class IdentityProfileAssemblyReference
{
    /// <summary>
    /// Gets the assembly that contains Wolverine handlers for the IdentityProfile bounded context.
    /// </summary>
    public static Assembly Assembly => typeof(CreateTrustInviteLink).Assembly;
}
