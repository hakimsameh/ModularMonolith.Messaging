using ModularMonolith.Messaging.Abstractions.Core;

namespace ModularMonolith.Messaging.UnitTest;

public class TrackingMiddleware<T> : IMessageMiddleware<T>
{
    public static List<string> Order = new();
    private readonly string _name;

    public TrackingMiddleware(string name)
    {
        _name = name;
    }

    public async Task InvokeAsync(T message, Func<Task> next)
    {
        Order.Add($"Before:{_name}");
        await next();
        Order.Add($"After:{_name}");
    }
}
