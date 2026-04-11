namespace BindingChaos.SharedKernel.Domain.Events;

/// <summary>
/// Marker interface for integration events that cross process boundaries and are transported via RabbitMQ.
/// Events implementing this interface are published to RabbitMQ in addition to the local in-process bus.
/// In-process BC-to-BC events should implement <see cref="IIntegrationEvent"/> only.
/// </summary>
public interface IExternalIntegrationEvent : IIntegrationEvent
{
}
