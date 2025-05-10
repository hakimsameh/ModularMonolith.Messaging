using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Messaging.Abstractions;
using ModularMonolith.Messaging.Internal;

namespace ModularMonolith.Messaging.UnitTest;

public class InMemoryBusTests
{
    private readonly IBus _bus;
    private static readonly string[] expected =
        [
        "Before:A", 
        "Before:B", 
        "Before:C",
        "After:C", 
        "After:B", 
        "After:A"
        ];

    public InMemoryBusTests()
    {
        var services = new ServiceCollection();

        services.AddScoped<IConsumer<HelloMessage>, HelloConsumer>();
        services.AddScoped<IMessageMiddleware<HelloMessage>>(sp => new TrackingMiddleware<HelloMessage>("A"));
        services.AddScoped<IMessageMiddleware<HelloMessage>>(sp => new TrackingMiddleware<HelloMessage>("B"));
        services.AddScoped<IMessageMiddleware<HelloMessage>>(sp => new TrackingMiddleware<HelloMessage>("C"));
        services.AddScoped<IMessageMiddleware<HelloMessage>, DummyMiddleware<HelloMessage>>();
        services.AddScoped<IMessageMiddleware<HelloMessage>, NonMatchingMiddleware<HelloMessage>>();
        services.AddScoped<IMessageMiddleware<HelloMessage>, BeforeOnlyMiddleware<HelloMessage>>();
        services.AddScoped<IMessageMiddleware<HelloMessage>, AfterOnlyMiddleware<HelloMessage>>();
        services.AddSingleton<IBus, InMemoryBus>();

        var provider = services.BuildServiceProvider();
        _bus = provider.GetRequiredService<IBus>();
    }

    [Fact]
    public async Task Should_Invoke_Consumer()
    {
        var msg = new HelloMessage { Text = "Test Message" };
        await _bus.SendAsync(msg);
        Assert.Equal("Test Message", HelloConsumer.LastMessage);
    }

    [Fact]
    public async Task Should_Invoke_Middleware()
    {
        DummyMiddleware<HelloMessage>.CalledBefore = false;
        DummyMiddleware<HelloMessage>.CalledAfter = false;
        await _bus.SendAsync(new HelloMessage { Text = "Ping" });
        Assert.True(DummyMiddleware<HelloMessage>.CalledBefore);
        Assert.True(DummyMiddleware<HelloMessage>.CalledAfter);
    }

    [Fact]
    public async Task Should_Not_Invoke_Unrelated_Middleware()
    {
        NonMatchingMiddleware<HelloMessage>.WasCalled = false;
        await _bus.SendAsync(new HelloMessage { Text = "Ignore" });
        Assert.False(NonMatchingMiddleware<HelloMessage>.WasCalled);
    }

    [Fact]
    public async Task Should_Throw_When_No_Consumer_Found()
    {
        var bus = new ServiceCollection()
            .AddSingleton<IBus, InMemoryBus>()
            .BuildServiceProvider()
            .GetRequiredService<IBus>();

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await bus.SendAsync(new HelloMessage { Text = "No Consumer" });
        });
    }

    [Fact]
    public async Task Should_Invoke_BeforeOnly_Middleware()
    {
        BeforeOnlyMiddleware<HelloMessage>.WasCalled = false;
        await _bus.SendAsync(new HelloMessage { Text = "Before" });
        Assert.True(BeforeOnlyMiddleware<HelloMessage>.WasCalled);
    }

    [Fact]
    public async Task Should_Invoke_AfterOnly_Middleware()
    {
        AfterOnlyMiddleware<HelloMessage>.WasCalled = false;
        await _bus.SendAsync(new HelloMessage { Text = "After" });
        Assert.True(AfterOnlyMiddleware<HelloMessage>.WasCalled);
    }

    [Fact]
    public async Task Should_Invoke_Middleware_In_Correct_Order()
    {
        TrackingMiddleware<HelloMessage>.Order.Clear();
        await _bus.SendAsync(new HelloMessage { Text = "Order Test" });
        Assert.Equal(
            expected,
            TrackingMiddleware<HelloMessage>.Order);
    }
}
