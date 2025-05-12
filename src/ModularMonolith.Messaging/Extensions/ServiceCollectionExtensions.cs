using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Messaging.Abstractions;
using ModularMonolith.Messaging.Internal;
using System.Reflection;

namespace ModularMonolith.Messaging.Extensions;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryMessaging(this IServiceCollection services, 
        params Assembly[] assemblies)
    {
        // Register Consumers
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

        services.AddSingleton<IBus, InMemoryBus>();
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
}
