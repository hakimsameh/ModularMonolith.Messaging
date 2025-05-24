namespace ModularMonolith.Messaging.Abstractions.Core;

public interface IEventMessage
{
    public Guid Id { get; }
    public DateTime OccurredOnUtc { get; }
}
