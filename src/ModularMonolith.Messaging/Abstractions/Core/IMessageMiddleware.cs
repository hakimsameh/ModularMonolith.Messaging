namespace ModularMonolith.Messaging.Abstractions.Core;

/// <summary>
/// Represents a middleware that can intercept messages of a specific type.
/// </summary>
/// <typeparam name="TMessage">The type of the message to intercept.</typeparam>
public interface IMessageMiddleware<TMessage>
{
    /// <summary>
    /// Invokes the middleware asynchronously.
    /// </summary>
    /// <param name="message">The message being processed.</param>
    /// <param name="next">The next middleware or consumer in the pipeline.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InvokeAsync(TMessage message, Func<Task> next);

    /// <summary>
    /// Gets the order in which the middleware should be executed. Lower values run first.
    /// </summary>
    int Order => 0;
}
