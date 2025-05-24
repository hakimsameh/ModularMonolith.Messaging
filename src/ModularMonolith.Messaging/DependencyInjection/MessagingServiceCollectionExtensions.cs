namespace ModularMonolith.Messaging.DependencyInjection;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddAsyncInMemoryMessaging(this IServiceCollection services,
        Action<MessageProcessingOptions>? configureOptions = null,
        params Assembly[] assemblies)
    {
        // Register consumer types
        foreach (var assembly in assemblies)
        {
            var consumerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
                    .Select(i => new { Interface = i, Implementation = t }))
                .ToList();

            foreach (var c in consumerTypes)
            {
                services.AddScoped(c.Interface, c.Implementation);
            }
        }

        // Configure options
        var options = new MessageProcessingOptions();
        configureOptions?.Invoke(options);
        services.TryAddSingleton(options);

        // Register the bus as a singleton
        services.TryAddSingleton<IBus, AsyncInMemoryBus>();
        services.TryAddSingleton<IEventPublisher, EventPublisher>();

        // Register middleware
        services.AddMessageMiddlewares(assemblies);

        return services;
    }

    private static IServiceCollection AddMessageMiddlewares(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var middlewareTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageMiddleware<>)))
            .ToList();

        foreach (var type in middlewareTypes)
        {
            var interfaceType = type.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageMiddleware<>));

            if (type.IsGenericTypeDefinition)
            {
                services.AddScoped(typeof(IMessageMiddleware<>), type);
            }
            else
            {
                services.AddScoped(interfaceType, type);
            }
        }

        return services;
    }
    public static IServiceCollection AddInMemoryMessaging(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var consumerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConsumer<>))
                    .Select(i => new { Interface = i, Implementation = t }))
                .ToList();

            foreach (var c in consumerTypes)
            {
                services.AddScoped(c.Interface, c.Implementation);
            }
        }

        services.TryAddSingleton<IBus, SyncInMemoryBus>();
        services.TryAddSingleton<IEventPublisher, EventPublisher>();
        services.AddMessageMiddlewares(assemblies);
        return services;
    }
}
