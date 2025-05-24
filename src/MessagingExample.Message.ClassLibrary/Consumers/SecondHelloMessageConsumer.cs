using MessagingExample.Message.ClassLibrary.Messages;
using Microsoft.Extensions.Logging;
using ModularMonolith.Messaging.Abstractions.Core;

namespace MessagingExample.Message.ClassLibrary.Consumers;

internal class SecondHelloMessageConsumer(ILogger<SecondHelloMessageConsumer> logger) : IConsumer<HelloMessage>
{
    public async Task ConsumeAsync(HelloMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine("------------------------------------------------------------");
        logger.LogInformation("Starting: {Consumer} Received: {Message}", GetType().Name, message.Text);
        Console.WriteLine($"[{GetType().Name}] Received: {message.Text}");
        await Task.Delay(1000, cancellationToken); // Simulate some work
        logger.LogInformation("Ending: {Consumer} Received: {Message}", GetType().Name, message.Text);
    }

}

