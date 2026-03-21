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

        var id = httpContext.User?.FindFirst(ParticipantIdClaimType)?.Value;

        return string.IsNullOrWhiteSpace(id) ? ParticipantId.Anonymous : ParticipantId.Create(id);
    }
}