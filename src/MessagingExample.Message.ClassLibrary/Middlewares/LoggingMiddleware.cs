using Microsoft.Extensions.Logging;
using ModularMonolith.Messaging.Abstractions.Core;
using System.Diagnostics;

namespace MessagingExample.Message.ClassLibrary.Middlewares;

internal class LoggingMiddleware<TMessage>(ILogger<LoggingMiddleware<TMessage>> logger) 
    : IMessageMiddleware<TMessage>
{
    private readonly ILogger<LoggingMiddleware<TMessage>> logger = logger;

    public async Task InvokeAsync(TMessage message, Func<Task> next)
    {
        logger.LogInformation(
            "Starting Message {@RequestName} Consumer On {@CurrentDateTime}",
            typeof(TMessage).Name, DateTime.UtcNow);
        var sw = Stopwatch.StartNew();
        await next();
        logger.LogInformation(
            "Message {@RequestName} Consumer Completed On {@CurrentDateTime} in {ms} ms",
            typeof(TMessage).Name, DateTime.UtcNow, sw.ElapsedMilliseconds);
        sw.Stop();
    }
}

