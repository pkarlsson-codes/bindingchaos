namespace BindingChaos.DocumentProcessing.Configuration;

/// <summary>
/// Strongly-typed options for messaging queue and exchange names.
/// </summary>
public sealed class MessagingOptions
{
    /// <summary>
    /// The RabbitMQ queue to consume MinIO attachment notifications from.
    /// </summary>
    public string AttachmentsQueue { get; set; } = "minio-attachments";

    /// <summary>
    /// The RabbitMQ exchange to publish platform events to.
    /// </summary>
    public string EventsExchange { get; set; } = "platform-events";
}
