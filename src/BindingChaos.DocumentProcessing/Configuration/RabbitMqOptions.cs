namespace BindingChaos.DocumentProcessing.Configuration;

/// <summary>
/// Strongly-typed options for the RabbitMQ message broker connection.
/// </summary>
public sealed class RabbitMqOptions
{
    /// <summary>
    /// RabbitMQ host name or IP address.
    /// </summary>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// RabbitMQ AMQP port.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// RabbitMQ username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// RabbitMQ password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
