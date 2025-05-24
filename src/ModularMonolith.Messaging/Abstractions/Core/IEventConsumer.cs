namespace ModularMonolith.Messaging.Abstractions.Core;

public interface IEventConsumer<TMessage> : IConsumer<TMessage>
    where TMessage : IEventMessage
{
    ///// <summary>
    ///// Consumes an event message asynchronously.
    ///// </summary>
    ///// <param name="message">The event message to consume.</param>
    ///// <param name="cancellationToken">The cancellation token.</param>
    ///// <returns>A task representing the asynchronous operation.</returns>
    //Task ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}