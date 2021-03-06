---
uid: outbound
---

# Outbound Connector

The outbound connector is used to automatically relay the integration messages (published to the internal bus) to the message broker. Multiple outbound endpoints can be configured and Silverback will route the messages according to their type (based on the `TMessage` parameter passed to the `AddOutbound<TMessage>` method.

## Implementations

Multiple implementations of the connector are available, offering a variable degree of reliability.

### Basic

The basic `OutboundConnector` is very simple and relays the messages synchronously. This is the easiest, better performing and most lightweight option but it doesn't allow for any transactionality (once the message is fired, is fired) nor resiliency to the message broker failure.

<figure>
	<a href="~/images/diagrams/outbound-basic.png"><img src="~/images/diagrams/outbound-basic.png"></a>
    <figcaption>Messages 1, 2 and 3 are directly produced to the message broker.</figcaption>
</figure>

# [Startup](#tab/basic-startup)
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSilverback()
            .WithConnectionToMessageBroker(options => options
                .AddKafka())
            .AddEndpointsConfigurator<MyEndpointsConfigurator>();
    }
}
```
# [EndpointsConfigurator](#tab/basic-configurator)
```csharp
public class MyEndpointsConfigurator : IEndpointsConfigurator
{
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder
            .AddOutbound<IIntegrationEvent>(
                new KafkaProducerEndpoint("basket-events")
                {
                    ...
                }));
    }
}
```
***

### Deferred

The `DeferredOutboundConnector` stores the messages into a local queue to be forwarded to the message broker in a separate step.

This approach has two main advantages:
1. Fault tollerance: you depend on the database only and if the message broker is unavailable the produce will be automatically retried later on
1. Transactionality: when using the database a storage you can commit the changes to the local database and the outbound messages inside a single atomic transaction (this pattern is called [transactional outbox](https://microservices.io/patterns/data/transactional-outbox.html))

<figure>
	<a href="~/images/diagrams/outbound-outboxtable.png"><img src="~/images/diagrams/outbound-outboxtable.png"></a>
    <figcaption>Messages 1, 2 and 3 are stored in the outbox table and produced by a separate thread or process.</figcaption>
</figure>

#### Database outbox table

The `DbOutboundConnector` will store the outbound messages into a database table.

When using entity framework (`UseDbContext<TDbContext>` contained in the `Silverback.EntityFrameworkCore` package) the outbound messages are stored into a `DbSet` and are therefore implicitly saved in the same transaction used to save all other changes.

> [!Note]
> The `DbContext` must include a `DbSet<OutboundMessage>` and an `OutboundWorker` is to be started to process the outbound queue. See also the <xref:dbcontext>.

> [!Important]
> The current `OutboundWorker` cannot be horizontally scaled and starting multiple instances will cause the messages to be produced multiple times. In the following example a distributed lock in the database is used to ensure that only one instance is running and another one will _immediatly_ take over when it stops (the `DbContext` must include a `DbSet<Lock>` as well, see also the <xref:dbcontext>). 

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSilverback()

            // Initialize Silverback to use MyDbContext as database storage.
            .UseDbContext<MyDbContext>()

            // Setup the lock manager using the database
            // to handle the distributed locks.
            // If this line is omitted the OutboundWorker will still
            // work without locking. 
            .AddDbDistributedLockManager()

            .WithConnectionToMessageBroker(options => options
                .AddKafka()
    
                // Use a deferred outbound connector
                .AddDbOutboundConnector()

                // Add the IHostedService processing the outbound queue
                // (overloads are available to specify custom interval,
                // lock timeout, etc.)
                .AddDbOutboundWorker())

            .AddEndpointsConfigurator<MyEndpointsConfigurator>();
    }
}
```

#### Custom outbound queue

You can easily create another implementation targeting another kind of storage, simply creating your own `IOutboundQueueWriter` and `IOutboundQueueReader` and plug them in.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSilverback()
            .AddDbDistributedLockManager()
            .WithConnectionToMessageBroker(options => options
                .AddKafka()
                .AddOutboundConnector<SomeCustomQueueWriter>()
                .AddOutboundWorker<SomeCustomQueueReader>())
            .AddEndpointsConfigurator<MyEndpointsConfigurator>();
    }
}
```

## Subscribing locally

The published messages that are routed to an outbound endpoint cannot be subscribed locally (within the same process), unless explicitely desired.

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSilverback()
            .AddDbDistributedLockManager()
            .WithConnectionToMessageBroker(options => options
                .AddKafka())
            .AddEndpointsConfigurator<MyEndpointsConfigurator>()
            .PublishOutboundMessagesToInternalBus();
    }
}
```

> [!Note]
> What said above is only partially true, as you can subscribe to the wrapped message (`IOutboundEnvelope<TMessage>`) even without calling `PublishOutboundMessagesToInternalBus`.

## Producing the same message to multiple endpoints

An outbound route can point to multiple endpoints resulting in every message being broadcasted to all endpoints.

<figure>
	<a href="~/images/diagrams/outbound-broadcast.png"><img src="~/images/diagrams/outbound-broadcast.png"></a>
    <figcaption>Messages 1, 2 and 3 are published to both topics simultaneously.</figcaption>
</figure>

```csharp
public class MyEndpointsConfigurator : IEndpointsConfigurator
{
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder
            .AddOutbound<IIntegrationCommand>(
                new KafkaProducerEndpoint("topic-1")
                {
                    ...
                },
                new KafkaProducerEndpoint("topic-2")
                {
                    ...
                }));
    }
}
```

A message will also be routed to all outbound endpoint mapped to a type compatible with the message type. In the example below an `OrderCreatedMessage` (that inherits from `OrderMessage`) would be sent to both endpoints.

```csharp
```csharp
public class MyEndpointsConfigurator : IEndpointsConfigurator
{
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder
            .AddOutbound<OrderMessage>(
                new KafkaProducerEndpoint("topic-1")
                {
                    ...
                })
            .AddOutbound<OrderCreatedMessage>(
                new KafkaProducerEndpoint("topic-1")
                {
                    ...
                }));
    }
}
```

## Dynamic custom routing

By default Silverback routes the messages according to their type and the static configuration defined at startup. In some cases you may need more flexibility, being able to apply your own routing rules. In such cases it is possible to implement a fully customized router.

<figure>
	<a href="~/images/diagrams/outbound-customrouting.png"><img src="~/images/diagrams/outbound-customrouting.png"></a>
    <figcaption>The messages are dynamically routed to the appropriate endpoint.</figcaption>
</figure>

In the following example a custom router is used to route the messages according to their priority (a copy is also sent to a catch-all topic).

# [Router](#tab/router)
```csharp
public class PrioritizedRouter : OutboundRouter<IPrioritizedCommand>
{
    private static readonly IProducerEndpoint HighPriorityEndpoint =
        new KafkaProducerEndpoint("high-priority")
        {
            ...
        };
    private static readonly IProducerEndpoint NormalPriorityEndpoint =
        new KafkaProducerEndpoint("normal-priority")
        {
            ...
        };
    private static readonly IProducerEndpoint LowPriorityEndpoint =
        new KafkaProducerEndpoint("low-priority")
        {
            ...
        };
    private static readonly IProducerEndpoint AllMessagesEndpoint =
        new KafkaProducerEndpoint("all")
        {
            ...
        };

    public override IEnumerable<IProducerEndpoint> Endpoints
    {
        get
        {
            yield return AllMessagesEndpoint;
            yield return LowPriorityEndpoint;
            yield return NormalPriorityEndpoint;
            yield return HighPriorityEndpoint;
        }
    }

    public override IEnumerable<IProducerEndpoint> GetDestinationEndpoints(
        IPrioritizedCommand message,
        MessageHeaderCollection headers)
    {
        yield return AllMessagesEndpoint;
        
        switch (message.Priority)
        {
            case MessagePriority.Low:
                yield return LowPriorityEndpoint;
                break;
            case MessagePriority.High:
                yield return HighPriorityEndpoint;
                break;
            default:
                yield return NormalPriorityEndpoint;
                break;
        }
    }
}
```
# [Startup](#tab/router-startup)
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSilverback()
            .WithConnectionToMessageBroker(options => options
                .AddKafka())
            .AddEndpointsConfigurator<MyEndpointsConfigurator>()
            .AddSingletonOutboundRouter<PrioritizedRouter>();
    }
}
```
# [EndpointsConfigurator](#tab/router-configurator)
```csharp
public class MyEndpointsConfigurator : IEndpointsConfigurator
{
    public void Configure(IEndpointsConfigurationBuilder builder)
    {
        builder.AddOutbound<IPrioritizedCommand, PrioritizedRouter>();
    }
}
```
***