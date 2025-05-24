namespace ModularMonolith.Messaging.Abstractions.Core;

/// <summary>
/// Represents a consumer that can handle messages of a specific type.
/// </summary>
/// <typeparam name="TMessage">The type of the message to consume.</typeparam>
public interface IConsumer<TMessage>
{
    /// <summary>
    /// Consumes a message asynchronously.
    /// </summary>
    /// <param name="message">The message to consume.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
}
