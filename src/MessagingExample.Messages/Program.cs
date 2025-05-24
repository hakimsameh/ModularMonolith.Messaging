using MessagingExample.Message.ClassLibrary;
using MessagingExample.Message.ClassLibrary.Events;
using MessagingExample.Message.ClassLibrary.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Messaging.Abstractions.Core;
using ModularMonolith.Messaging.DependencyInjection;
using Serilog;
using System.Reflection;

var logger = Log.Logger = new LoggerConfiguration()
                          .Enrich.FromLogContext()
                          .WriteTo.Console()
                          .CreateLogger();
logger.Information("Starting Application");
var builder = WebApplication.CreateBuilder(args);
List<Assembly> assemblies = [typeof(Program).Assembly];
var services = builder.Services;
//services.AddLogging(configure => configure.AddConsole());
services.AddMessageExample(assemblies);



// synchronous in-memory bus
//services.AddInMemoryMessaging(assemblies.ToArray());

// async in-memory bus with options
services.AddAsyncInMemoryMessaging(options =>
{
    options.MaxConcurrentMessages = 10;
    options.MaxQueueSize = 500;
    options.MaxRetries = 5;
    options.RetryDelay = TimeSpan.FromSeconds(2);
}, assemblies.ToArray());

// async in-memory bus without options
//services.AddAsyncInMemoryMessaging(assemblies: assemblies.ToArray());


var provider = services.BuildServiceProvider();
var bus = provider.GetRequiredService<IBus>();
var eventPublisher = provider.GetRequiredService<IEventPublisher>();

await eventPublisher.PublishAsync(new EventMessageTest(" Event Hello from main!"));
Console.WriteLine("Publishing HelloMessage...");
await bus.SendAsync(new HelloMessage { Text = "Hello from main!" });
logger.Information("Finished");
Console.WriteLine("-------------------------------------------------");
Console.WriteLine("Publishing Another HelloMessage...");
await bus.SendAsync(new HelloMessage { Text = "Another Message!" });
logger.Information("Finished");
Console.ReadKey();
