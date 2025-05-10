using MessagingExample.Message.ClassLibrary.Messages;
using ModularMonolith.Messaging.Abstractions;

namespace MessagingExample.Message.ClassLibrary.Consumers;

internal class SecondHelloMessageConsumer : IConsumer<HelloMessage>
{
    public Task ConsumeAsync(HelloMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Second Consumer] Received: {message.Text}");
        return Task.CompletedTask;
    }

}

