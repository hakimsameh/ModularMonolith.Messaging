namespace ModularMonolith.Messaging.Abstractions;

public interface IConsumer<TMessage>
{
    Task ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}
