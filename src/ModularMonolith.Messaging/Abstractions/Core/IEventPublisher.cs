namespace ModularMonolith.Messaging.Abstractions.Core;

public interface IEventPublisher
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) 
        where TMessage : IEventMessage;
}
