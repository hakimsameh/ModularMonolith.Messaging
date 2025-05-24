namespace ModularMonolith.Messaging.Abstractions.Infrastructure;

internal class EventPublisher(IBus bus) : IEventPublisher
{
    private readonly IBus _bus = bus;

    public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IEventMessage
    {
        return _bus.SendAsync(message, cancellationToken);
    }
}