namespace BindingChaos.Web.Gateway.Models;

/// <summary>
/// Generic API response envelope that wraps data with metadata.
/// </summary>
/// <typeparam name="T">The type of data being wrapped.</typeparam>
internal sealed class ApiEnvelope<T>
{
    /// <summary>
    /// Unique identifier for the request.
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the response was generated.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// The actual data payload.
    /// </summary>
    public T Data { get; set; } = default!;
}