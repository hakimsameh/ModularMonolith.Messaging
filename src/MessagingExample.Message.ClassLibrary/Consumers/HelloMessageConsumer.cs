using MessagingExample.Message.ClassLibrary.Messages;
using ModularMonolith.Messaging.Abstractions;

namespace MessagingExample.Message.ClassLibrary.Consumers;

internal class HelloMessageConsumer : IConsumer<HelloMessage>
{
    public Task ConsumeAsync(HelloMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Consumer] Received: {message.Text}");
        return Task.CompletedTask;
    }

}
