namespace ModularMonolith.Messaging.Abstractions;

public interface IMessageMiddleware<TMessage>
{
    Task InvokeAsync(TMessage message, Func<Task> next);
    int Order => 0;
}

