using ModularMonolith.Messaging.Abstractions;

namespace ModularMonolith.Messaging.UnitTest;

public class HelloConsumer : IConsumer<HelloMessage>
{
    public static string? LastMessage;

    public Task ConsumeAsync(HelloMessage message, CancellationToken cancellationToken)
    {
        LastMessage = message.Text;
        return Task.CompletedTask;
    }
}
