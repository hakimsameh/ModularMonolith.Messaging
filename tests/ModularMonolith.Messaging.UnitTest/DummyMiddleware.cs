using ModularMonolith.Messaging.Abstractions.Core;
//using ModularMonolith.Messaging.Attributes;

namespace ModularMonolith.Messaging.UnitTest;

[MiddlewareTarget(typeof(HelloMessage))]
public class DummyMiddleware<T> : IMessageMiddleware<T>
{
    public static bool CalledBefore;
    public static bool CalledAfter;

    public async Task InvokeAsync(T message, Func<Task> next)
    {
        CalledBefore = true;
        await next();
        CalledAfter = true;
    }
}
