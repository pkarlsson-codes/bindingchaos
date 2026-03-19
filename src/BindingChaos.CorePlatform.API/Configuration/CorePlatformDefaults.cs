namespace BindingChaos.CorePlatform.API.Configuration;

/// <summary>
/// Default configuration values for the Core Platform API application.
/// Centralizes magic numbers and fallback values.
/// </summary>
internal static class CorePlatformDefaults
{
    /// <summary>
    /// Database configuration constants.
    /// </summary>
    public static class Database
    {
        /// <summary>
        /// Configuration key for database connection string lookup.
        /// </summary>
        public const string ConfigurationKey = "Database";

        /// <summary>
        /// Database connection timeout in seconds.
        /// </summary>
        public const int ConnectionTimeoutSeconds = 30;
    }

    /// <summary>
    /// JWT authentication configuration constants.
    /// </summary>
    public static class Authentication
    {
        /// <summary>
        /// Configuration key for JWT issuer.
        /// </summary>
        public const string IssuerConfigKey = "InternalJwt:Issuer";

        /// <summary>
        /// Configuration key for JWT audience.
        /// </summary>
        public const string AudienceConfigKey = "InternalJwt:Audience";

        /// <summary>
        /// Configuration key for JWT signing key.
        /// </summary>
        public const string SigningKeyConfigKey = "InternalJwt:SigningKey";

        /// <summary>
        /// Environment variable name for JWT secret.
        /// </summary>
        public const string JwtSecretEnvironmentVariable = "BINDINGCHAOS_INTERNAL_JWT_SECRET";

        /// <summary>
        /// Default JWT issuer.
        /// </summary>
        public const string DefaultIssuer = "BindingChaos.Gateway";

        /// <summary>
        /// Default JWT audience.
        /// </summary>
        public const string DefaultAudience = "BindingChaos.CorePlatform";

        /// <summary>
        /// Development JWT secret (only used in development).
        /// </summary>
        public const string DevelopmentSecret = "dev-internal-jwt-secret-change-me";

        /// <summary>
        /// Participant ID claim name.
        /// </summary>
        public const string ParticipantIdClaim = "participant_id";

        /// <summary>
        /// JWT clock skew tolerance.
        /// </summary>
        public static readonly TimeSpan ClockSkew = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Health check configuration constants.
    /// </summary>
    public static class HealthChecks
    {
        /// <summary>
        /// Self health check name.
        /// </summary>
        public const string SelfCheckName = "self";

        /// <summary>
        /// Health check endpoint path.
        /// </summary>
        public const string EndpointPath = "/health";
    }

    /// <summary>
    /// Application-wide constants.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Application display name.
        /// </summary>
        public const string Name = "BindingChaos Core Platform API";
    }
}