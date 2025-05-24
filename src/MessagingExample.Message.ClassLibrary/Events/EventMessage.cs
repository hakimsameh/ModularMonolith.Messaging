using ModularMonolith.Messaging.Abstractions.Core;

namespace MessagingExample.Message.ClassLibrary.Events;

public class EventMessageTest(string text) : EventMessageBase()
{
    public string Text { get; } = text;
}
