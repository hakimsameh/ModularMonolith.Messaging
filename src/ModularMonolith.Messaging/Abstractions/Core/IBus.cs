namespace ModularMonolith.Messaging.Abstractions.Core;
/// <summary>
/// Represents the core messaging service for sending messages.
/// </summary>
public interface IBus
{
    /// <summary>
    /// Sends a message asynchronously.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default);
}