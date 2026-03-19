namespace BindingChaos.CorePlatform.API.Configuration;

/// <summary>
/// Strongly-typed options for the MinIO object storage client.
/// </summary>
public sealed class MinioOptions
{
    /// <summary>
    /// MinIO server endpoint (host:port).
    /// </summary>
    public string Endpoint { get; set; } = "localhost:9000";

    /// <summary>
    /// MinIO access key (username).
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// MinIO secret key (password).
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Whether to use SSL for MinIO connections.
    /// </summary>
    public bool UseSsl { get; set; }
}
