# ModularMonolith.Messaging

A robust, scalable messaging infrastructure designed for modular monolithic applications.

## Overview

ModularMonolith.Messaging provides a lightweight yet powerful messaging system that enables communication between modules in a modular monolith architecture. It offers the benefits of service isolation while avoiding the complexity and overhead of a full microservices implementation.

## Features

- **Decoupled Module Communication**: Enables modules to communicate without direct dependencies
- **Type-Safe Message Handling**: Strong typing for message contracts and handlers
- **In-Process Messaging**: High-performance in-memory message passing
- **Support for Integration Events**: Facilitate integration with external systems
- **Extensible Architecture**: Designed for easy addition of new message types and handlers

## Installation

Install the package via NuGet:

```shell
dotnet add package ModularMonolith.Messaging
```

## Basic Usage

### 1. Define a Message (Contract Project)

```csharp

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
}
```

### 2. Modules 

- **Cart Module**: Create Cart.
- **Order Module**: Create Order.
- **Customer Module**: Customer Orders.
- **Payment Module**: Customer Payment Data.
- **Contract**: Has Contracts (Shared Project).
- **WebApi**: Web Api Project.

### 3. In Every Modules Has This Static Class

```csharp

internal static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
    public static readonly string ModuleName = typeof(AssemblyReference).Assembly.GetName().Name!;
}

```

Use It To Get Assembly And ModuleName


### 4. In Cart Module

```csharp

public interface IOrderService
{
    Task<Guid> CreateOrder(string customerName, decimal totalAmount);
}

```

```csharp

internal sealed class OrderService : IOrderService
{
    private readonly IBus _bus;
    public OrderService(IBus bus) => _bus = bus;
    public async Task<Guid> CreateOrder(string customerName, decimal totalAmount)
    {
        var orderId = Guid.NewGuid();
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            CreatedAt = DateTime.UtcNow,
            CustomerName = customerName,
            TotalAmount = totalAmount
        };

        // Add Any Another Logic

        await _bus.SendAsync(orderCreatedEvent);
        return orderId;
    }
}

```

- **OrderService** Will sent *OrderCreatedEvent* to all modules that are interested in this Event throw *SendAsync* Method.

### 5. Adding Consumer To Modules

```csharp

internal class OrderCreatedEventConsumer(ILogger<OrderCreatedEvent> logger) : IConsumer<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEvent> _logger = logger;

    public Task ConsumeAsync(OrderCreatedEvent message, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting Message {@RequestName} Consumer On {@CurrentDateTime} on {Module}",
            typeof(OrderCreatedEvent).Name, DateTime.UtcNow, AssemblyReference.ModuleName);
        //Console.WriteLine($"{AssemblyReference.ModuleName} Received: From Customer: {message.CustomerName}");
        //Console.WriteLine($"{AssemblyReference.ModuleName} Received: From Total Amount: {message.TotalAmount}");
        //Console.WriteLine($"{AssemblyReference.ModuleName} Received: From Order Id: {message.OrderId}");
        //Console.WriteLine($"{AssemblyReference.ModuleName} Received: From Created At: {message.CreatedAt}");

        // Add Any Logic

        _logger.LogInformation(
            "Ending Message {@RequestName} Consumer On {@CurrentDateTime} on {Module}",
            typeof(OrderCreatedEvent).Name, DateTime.UtcNow, AssemblyReference.ModuleName);
        return Task.CompletedTask;
    }
}


```

### 6. Add Modules Extension Methods 

```csharp
    public static IServiceCollection AddCustomerModule(this IServiceCollection services, List<Assembly> assemblies)
    {
        assemblies.Add(AssemblyReference.Assembly);
        return services;
    }
```

### 7. In Web Api 

- **Program.cs**

```csharp

var assemblies = new List<Assembly> { typeof(Program).Assembly };

// Add Modules Services
builder.Services.AddCartModule(assemblies);
builder.Services.AddCustomerModule(assemblies);
builder.Services.AddOrderModule(assemblies);
builder.Services.AddPaymentModule(assemblies);


// Add Messaging Service (sync in-memory bus)
//builder.Services.AddInMemoryMessaging(assemblies.ToArray());


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


.
.
.

app.MapPost("/order", async (IOrderService service, OrderRequest request) =>
{
    var result = await service.CreateOrder(request.CustomerName, request.TotalAmount);
    return Results.Ok(result);
});

app.Run();



public record OrderRequest(string CustomerName, decimal TotalAmount);

```

### 8. Download

You Can Download Project Source Code [GitHub Page](https://github.com/hakimsameh/ModularMonolith.Messaging.Solution).


## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

If you encounter any issues or have questions, please file an issue on the GitHub repository.
