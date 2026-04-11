using BindingChaos.SharedKernel.Domain.Events;
using Wolverine.Configuration;
using Wolverine.RabbitMQ.Internal;
using Wolverine.Runtime;
using Wolverine.Runtime.Routing;

namespace BindingChaos.CorePlatform.API.Infrastructure.Messaging;

/// <summary>
/// Routes messages implementing <see cref="IExternalIntegrationEvent"/> to a RabbitMQ queue
/// whose name is derived from the message type name in kebab-case.
/// Combined with <c>opts.Policies.ConventionalLocalRoutingIsAdditive()</c>, external integration
/// events are delivered both in-process (for local BC handlers) and to RabbitMQ (for external services).
/// Events that only implement <see cref="IIntegrationEvent"/> route locally only.
/// </summary>
internal sealed class ExternalIntegrationEventRoutingConvention : IMessageRoutingConvention
{
    /// <inheritdoc/>
    public void DiscoverListeners(IWolverineRuntime runtime, IReadOnlyList<Type> handledMessageTypes)
    {
        // Inbound RabbitMQ listeners for events originating from external services
        // are registered explicitly in Program.cs, because the source queue name
        // cannot be derived from the consuming handler type alone.
    }

    /// <inheritdoc/>
    public IEnumerable<Wolverine.Configuration.Endpoint> DiscoverSenders(Type messageType, IWolverineRuntime runtime)
    {
        if (!typeof(IExternalIntegrationEvent).IsAssignableFrom(messageType))
        {
            yield break;
        }

        var transport = runtime.Options.Transports.GetOrCreate<RabbitMqTransport>();
        var queue = transport.Queues[ToQueueName(messageType)];
        var sendingAgent = runtime.Endpoints.GetOrBuildSendingAgent(queue.Uri);
        yield return sendingAgent.Endpoint;
    }

    /// <summary>
    /// Derives the RabbitMQ queue name for a message type using kebab-case of the type name.
    /// E.g. <c>SignalCapturedIntegrationEvent</c> → <c>signal-captured-integration-event</c>.
    /// </summary>
    /// <param name="messageType">The message type to derive the queue name from.</param>
    /// <returns>The kebab-case queue name.</returns>
    internal static string ToQueueName(Type messageType) =>
        string.Concat(messageType.Name.Select((c, i) =>
            i > 0 && char.IsUpper(c)
                ? $"-{char.ToLowerInvariant(c)}"
                : char.ToLowerInvariant(c).ToString()));
}
