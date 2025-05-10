namespace ModularMonolith.Messaging.Abstractions;
public interface IBus
{
    Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
}