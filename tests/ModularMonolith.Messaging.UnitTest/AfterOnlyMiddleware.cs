using ModularMonolith.Messaging.Abstractions;
using ModularMonolith.Messaging.Attributes;

namespace ModularMonolith.Messaging.UnitTest;

[MiddlewareTarget(typeof(HelloMessage))]
public class AfterOnlyMiddleware<T> : IMessageMiddleware<T>
{
    public static bool WasCalled;

    public async Task InvokeAsync(T message, Func<Task> next)
    {
        await next();
        WasCalled = true;
    }
}
