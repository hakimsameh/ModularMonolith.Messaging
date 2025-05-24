using ModularMonolith.Messaging.Abstractions.Core;

namespace ModularMonolith.Messaging.UnitTest;

[MiddlewareTarget(typeof(HelloMessage))]
public class BeforeOnlyMiddleware<T> : IMessageMiddleware<T>
{
    public static bool WasCalled;

    public async Task InvokeAsync(T message, Func<Task> next)
    {
        WasCalled = true;
        await next();
    }
}
