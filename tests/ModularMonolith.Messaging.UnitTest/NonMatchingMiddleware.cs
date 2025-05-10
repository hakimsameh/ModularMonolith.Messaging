using ModularMonolith.Messaging.Abstractions;
using ModularMonolith.Messaging.Attributes;

namespace ModularMonolith.Messaging.UnitTest;

[MiddlewareTarget(typeof(UnrelatedMessage))]
public class NonMatchingMiddleware<T> : IMessageMiddleware<T>
{
    public static bool WasCalled;

    public NonMatchingMiddleware()
    {
    }

    public async Task InvokeAsync(T message, Func<Task> next)
    {
        WasCalled = true;
        await next();
    }
}
