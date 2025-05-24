namespace ModularMonolith.Messaging.Abstractions.Core;

public abstract class EventMessageBase(Guid id, DateTime occurredOnUtc) : IEventMessage
{
    protected EventMessageBase() 
        : this(Guid.NewGuid(), DateTime.UtcNow)
    {
    }
    public Guid Id { get; init; } = id;
    public DateTime OccurredOnUtc { get; init; } = occurredOnUtc;
}