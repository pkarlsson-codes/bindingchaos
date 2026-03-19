using System.Security.Claims;
using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.CorePlatform.API.Infrastructure.Extensions;

/// <summary>
/// Extensions for resolving participant identity from HttpContext in APIs.
/// </summary>
internal static class HttpContextParticipantExtensions
{
    private const string ParticipantIdClaimType = "participant_id";

    /// <summary>
    /// Resolves the current participant id from claims, or returns ParticipantId.Anonymous.
    /// </summary>
    /// <param name="httpContext">The HttpContext.</param>
    /// <returns>The ParticipantId if available; otherwise ParticipantId.Anonymous.</returns>
    public static ParticipantId GetParticipantIdOrAnonymous(this HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var id = httpContext.User?.FindFirst(ParticipantIdClaimType)?.Value
                 ?? httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? httpContext.User?.Identity?.Name;

        if (string.IsNullOrWhiteSpace(id) || id == "anonymous")
        {
            return ParticipantId.Anonymous;
        }

        return ParticipantId.Create(id);
    }
}