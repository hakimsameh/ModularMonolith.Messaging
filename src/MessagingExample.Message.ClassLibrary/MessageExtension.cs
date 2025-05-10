using MessagingExample.Message.ClassLibrary.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace MessagingExample.Message.ClassLibrary;

public static class MessageExtension
{
    public static IServiceCollection AddMessageExample(this IServiceCollection services, List<Assembly> assemblies)
    {
        services.AddLogging();
        assemblies.Add(typeof(MessageExtension).Assembly);
        //services.AddScoped(typeof(IMessageMiddleware<>), typeof(LoggingMiddleware<>));
        
        return services;
    }
}
