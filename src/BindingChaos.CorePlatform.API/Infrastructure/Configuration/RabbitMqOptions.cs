namespace BindingChaos.CorePlatform.API.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed options for the RabbitMQ message broker connection.
/// </summary>
internal sealed class RabbitMqOptions
{
    /// <summary>RabbitMQ host name or IP address.</summary>
    public string Host { get; set; } = "localhost";

    /// <summary>RabbitMQ AMQP port.</summary>
    public int Port { get; set; } = 5672;

    /// <summary>RabbitMQ username.</summary>
    public string Username { get; set; } = "guest";

    /// <summary>RabbitMQ password.</summary>
    public string Password { get; set; } = "guest";
}
