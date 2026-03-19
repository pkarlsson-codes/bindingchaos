namespace BindingChaos.Web.Gateway.Services;

/// <summary>
/// Helper service for detecting client IP addresses from HTTP requests.
/// </summary>
public static partial class IpAddressHelper
{
    /// <summary>
    /// Gets the client IP address from the request headers and connection.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <param name="logger">The logger for debugging.</param>
    /// <returns>The client IP address, or null if not found.</returns>
    public static string? GetClientIpAddress(HttpContext httpContext, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var clientIp = forwardedFor.Split(',')[0].Trim();
            if (logger != null)
            {
                Logs.DetectedIpFromForwardedFor(logger, clientIp);
            }

            return clientIp;
        }

        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            if (logger != null)
            {
                Logs.DetectedIpFromRealIp(logger, realIp);
            }

            return realIp;
        }

        var connectionIp = httpContext.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrWhiteSpace(connectionIp))
        {
            if (logger != null)
            {
                Logs.DetectedIpFromConnection(logger, connectionIp);
            }

            return connectionIp;
        }

        if (logger != null)
        {
            Logs.CouldNotDetectClientIp(logger);
        }

        return null;
    }

    /// <summary>
    /// Validates if the provided IP address is in a valid format.
    /// </summary>
    /// <param name="ipAddress">The IP address to validate.</param>
    /// <returns>True if the IP address is valid; otherwise, false.</returns>
    public static bool IsValidIpAddress(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return false;
        }

        if (System.Net.IPAddress.TryParse(ipAddress, out var parsedIp))
        {
            return parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ||
                   parsedIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
        }

        return false;
    }

    private static partial class Logs
    {
        [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Detected client IP from X-Forwarded-For header: {ClientIp}")]
        internal static partial void DetectedIpFromForwardedFor(ILogger logger, string clientIp);

        [LoggerMessage(EventId = 2, Level = LogLevel.Debug, Message = "Detected client IP from X-Real-IP header: {ClientIp}")]
        internal static partial void DetectedIpFromRealIp(ILogger logger, string clientIp);

        [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Detected client IP from connection: {ClientIp}")]
        internal static partial void DetectedIpFromConnection(ILogger logger, string clientIp);

        [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "Could not detect client IP address from request")]
        internal static partial void CouldNotDetectClientIp(ILogger logger);
    }
}
