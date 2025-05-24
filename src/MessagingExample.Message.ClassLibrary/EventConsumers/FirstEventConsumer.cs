using MessagingExample.Message.ClassLibrary.Events;
using ModularMonolith.Messaging.Abstractions.Core;

namespace MessagingExample.Message.ClassLibrary.EventConsumers;

internal class FirstEventConsumer : IEventConsumer<EventMessageTest>
{
    public Task ConsumeAsync(EventMessageTest message, CancellationToken cancellationToken)
    {
        // Handle the event message
        Console.WriteLine($"FirstEventConsumer: {message.Text}");
        return Task.CompletedTask;
    }
}

