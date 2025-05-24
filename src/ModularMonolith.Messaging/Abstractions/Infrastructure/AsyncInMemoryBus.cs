namespace ModularMonolith.Messaging.Abstractions.Infrastructure;

internal class AsyncInMemoryBus : IBus, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MessageProcessingOptions _options;
    private readonly ConcurrentQueue<MessageEnvelope> _messageQueue;
    private readonly SemaphoreSlim _queueSignal;
    private readonly SemaphoreSlim _throttler;
    private readonly CancellationTokenSource _shutdownToken;
    private readonly Task _processingTask;
    private bool _disposed;

    public AsyncInMemoryBus(IServiceProvider serviceProvider, MessageProcessingOptions? options = null)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _options = options ?? new MessageProcessingOptions();
        _messageQueue = new ConcurrentQueue<MessageEnvelope>();
        _queueSignal = new SemaphoreSlim(0);
        _throttler = new SemaphoreSlim(_options.MaxConcurrentMessages);
        _shutdownToken = new CancellationTokenSource();
        _processingTask = Task.Run(ProcessQueueAsync);
    }

    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(AsyncInMemoryBus));

        if (message == null)
            throw new ArgumentNullException(nameof(message));

        var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, _shutdownToken.Token).Token;

        // Create an envelope to hold the message and its type
        var envelope = new MessageEnvelope(message, typeof(TMessage), combinedToken);

        // Wait until there's room in the queue if it's bounded
        if (_messageQueue.Count >= _options.MaxQueueSize)
        {
            await Task.Yield(); // Ensure we don't block the caller synchronously

            while (_messageQueue.Count >= _options.MaxQueueSize && !combinedToken.IsCancellationRequested)
            {
                await Task.Delay(10, combinedToken);
            }
        }

        // Add to queue and signal the processing task
        _messageQueue.Enqueue(envelope);
        _queueSignal.Release();
    }

    private async Task ProcessQueueAsync()
    {
        while (!_shutdownToken.IsCancellationRequested)
        {
            try
            {
                // Wait for a message to be available
                await _queueSignal.WaitAsync(_shutdownToken.Token);

                // Dequeue the message
                if (!_messageQueue.TryDequeue(out var envelope))
                    continue;

                // Throttle concurrent processing
                await _throttler.WaitAsync(_shutdownToken.Token);

                // Process the message asynchronously
                _ = ProcessMessageAsync(envelope)
                    .ContinueWith(_ => _throttler.Release(), TaskContinuationOptions.ExecuteSynchronously);
            }
            catch (OperationCanceledException) when (_shutdownToken.IsCancellationRequested)
            {
                // Normal shutdown
                break;
            }
            catch (Exception ex)
            {
                // Log exception but continue processing
                Console.WriteLine($"Error in message queue processing: {ex}");
            }
        }
    }

    private async Task ProcessMessageAsync(MessageEnvelope envelope)
    {
        var retryCount = 0;
        var processed = false;

        while (!processed && retryCount <= _options.MaxRetries && !envelope.CancellationToken.IsCancellationRequested)
        {
            try
            {
                // Use reflection to call the generic method with the correct type
                var method = typeof(AsyncInMemoryBus).GetMethod(nameof(ProcessTypedMessageAsync),
                    BindingFlags.NonPublic | BindingFlags.Instance) 
                    ?? throw new InvalidOperationException($"Method {nameof(ProcessTypedMessageAsync)} not found.");
                var genericMethod = method.MakeGenericMethod(envelope.MessageType);
                var task = (Task)genericMethod.Invoke(this, [envelope.Message, envelope.CancellationToken])!;

                await task;
                processed = true;
            }
            catch (Exception ex) when (_options.SwallowExceptions)
            {
                retryCount++;

                if (retryCount <= _options.MaxRetries)
                {
                    Console.WriteLine($"Error processing message (retry {retryCount}/{_options.MaxRetries}): {ex.Message}");
                    await Task.Delay(_options.RetryDelay, envelope.CancellationToken);
                }
                else
                {
                    Console.WriteLine($"Failed to process message after {_options.MaxRetries} retries: {ex}");
                }
            }
        }
    }

    private async Task ProcessTypedMessageAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var middlewares = scopedProvider
            .GetServices<IMessageMiddleware<TMessage>>()
            .Where(m => ShouldRunMiddleware(m, message))
            .OrderBy(m => m.Order)
            .ToList();

        Func<Task> pipeline = () => InvokeConsumer(message, scopedProvider, cancellationToken);

        foreach (var middleware in middlewares.AsEnumerable().Reverse())
        {
            var next = pipeline;
            pipeline = () => middleware.InvokeAsync(message, next);
        }

        await pipeline();
    }

    private static async Task InvokeConsumer<TMessage>(TMessage message, IServiceProvider provider, CancellationToken cancellationToken)
    {
        var consumers = provider.GetServices<IConsumer<TMessage>>().ToList();
        if (consumers == null || !consumers.Any())
            throw new InvalidOperationException($"No consumer registered for message type {typeof(TMessage).Name}");

        // Process all consumers in parallel
        await Task.WhenAll(consumers.Select(consumer => consumer.ConsumeAsync(message, cancellationToken)));
    }

    private static bool ShouldRunMiddleware<TMessage>(IMessageMiddleware<TMessage> middleware, TMessage message)
    {
        var attr = middleware.GetType().GetCustomAttribute<MiddlewareTargetAttribute>();
        if (attr == null) return true;
        return attr.TargetMessages.Any(t => t == typeof(TMessage));
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _shutdownToken.Cancel();

        try
        {
            // Wait for the processing task to complete with a timeout
            _processingTask.Wait(TimeSpan.FromSeconds(5));
        }
        catch (Exception)
        {
            // Ignore exceptions during shutdown
        }

        _shutdownToken.Dispose();
        _queueSignal.Dispose();
        _throttler.Dispose();
    }

    private class MessageEnvelope(object message, Type messageType, CancellationToken cancellationToken)
    {
        public object Message { get; } = message;
        public Type MessageType { get; } = messageType;
        public CancellationToken CancellationToken { get; } = cancellationToken;
    }
}
