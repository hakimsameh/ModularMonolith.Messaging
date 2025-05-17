using MessagingExample.Message.ClassLibrary;
using MessagingExample.Message.ClassLibrary.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ModularMonolith.Messaging.Abstractions;
using ModularMonolith.Messaging.Extensions;
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
services.AddInMemoryMessaging(assemblies.ToArray());
var provider = services.BuildServiceProvider();
var bus = provider.GetRequiredService<IBus>();

Console.WriteLine("Publishing HelloMessage...");
await bus.SendAsync(new HelloMessage { Text = "Hello from main!" });
logger.Information("Finished");
Console.WriteLine("-------------------------------------------------");
Console.WriteLine("Publishing Another HelloMessage...");
await bus.SendAsync(new HelloMessage { Text = "Another Message!" });
logger.Information("Finished");
Console.ReadKey();
