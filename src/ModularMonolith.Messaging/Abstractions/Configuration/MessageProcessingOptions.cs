namespace ModularMonolith.Messaging.Abstractions.Configuration;

/// <summary>
/// Options for configuring message processing behavior.
/// </summary>
public class MessageProcessingOptions
{
    /// <summary>
    /// Gets or sets the maximum number of messages that can be processed concurrently.
    /// Default is Environment.ProcessorCount * 2.
    /// </summary>
    public int MaxConcurrentMessages { get; set; } = Environment.ProcessorCount * 2;

    /// <summary>
    /// Gets or sets the maximum queue size before sending operations start blocking.
    /// Default is 1000.
    /// </summary>
    public int MaxQueueSize { get; set; } = 1000;

    /// <summary>
    /// Gets or sets a value indicating whether exceptions in message handling should be swallowed.
    /// Default is true.
    /// </summary>
    public bool SwallowExceptions { get; set; } = true;

    /// <summary>
    /// Gets or sets the time to wait between retries for failed messages.
    /// Default is 1 second.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the maximum number of retries for failed messages.
    /// Default is 3.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
}
