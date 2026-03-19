namespace BindingChaos.Web.Gateway.Configuration;

/// <summary>
/// Default configuration values for the Gateway application.
/// Centralizes magic numbers and fallback values.
/// </summary>
internal static class GatewayDefaults
{
    /// <summary>
    /// Default CORS allowed origins for development.
    /// </summary>
    public static readonly string[] DevelopmentCorsOrigins = [
        "http://localhost:3000",
        "https://localhost:3000"
    ];

    /// <summary>
    /// Cookie names used by the Gateway.
    /// </summary>
    public static class Cookies
    {
        /// <summary>
        /// Authentication cookie name.
        /// </summary>
        public const string AuthenticationCookie = "bc_auth";

        /// <summary>
        /// Session cookie name.
        /// </summary>
        public const string SessionCookie = "bc_session";

        /// <summary>
        /// CSRF protection cookie name.
        /// </summary>
        public const string CsrfCookie = "bc_csrf";
    }

    /// <summary>
    /// Default timeouts and limits.
    /// </summary>
    public static class Timeouts
    {
        /// <summary>
        /// Default HTTP client timeout.
        /// </summary>
        public static readonly TimeSpan HttpClientTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Default authentication session timeout.
        /// </summary>
        public static readonly TimeSpan AuthenticationTimeout = TimeSpan.FromMinutes(20);
    }

    /// <summary>
    /// Application-wide constants.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Application display name.
        /// </summary>
        public const string Name = "BindingChaos Web Gateway";

        /// <summary>
        /// Startup log message.
        /// </summary>
        public const string StartupMessage = "Starting BindingChaos Web Gateway";

        /// <summary>
        /// Startup completion log message.
        /// </summary>
        public const string StartupCompleteMessage = "Gateway startup completed successfully";

        /// <summary>
        /// Unexpected termination log message.
        /// </summary>
        public const string UnexpectedTerminationMessage = "Gateway application terminated unexpectedly";
    }
}