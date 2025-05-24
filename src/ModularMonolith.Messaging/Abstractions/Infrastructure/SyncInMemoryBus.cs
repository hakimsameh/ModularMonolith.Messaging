using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Messaging.Abstractions.Core;
using System.Reflection;

namespace ModularMonolith.Messaging.Abstractions.Infrastructure;

internal class SyncInMemoryBus(IServiceProvider serviceProvider) : IBus
{

    public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
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
        var consumers = provider.GetServices<IConsumer<TMessage>>();
        if (consumers == null || !consumers.Any())
            throw new InvalidOperationException($"No consumer registered for message type {typeof(TMessage).Name}");
        foreach (var consumer in consumers)
        {
            await consumer.ConsumeAsync(message, cancellationToken);
        }
    }

    private static bool ShouldRunMiddleware<TMessage>(IMessageMiddleware<TMessage> middleware, TMessage message)
    {
        var attr = middleware.GetType().GetCustomAttribute<MiddlewareTargetAttribute>();
        if (attr == null) return true;
        return attr.TargetMessages.Any(t => t == typeof(TMessage));
    }
}