---
uid: translating-messages
---

# Translating Messages

It is not uncommon to be willing to slightly transform the internal message before exporting it to the outside world (e.g. you may not want to export the full entity related to the domain event). You can easily achieve this with a subscriber that just maps/translates the messages.

```csharp
public class MapperService
{
    public IMessage MapCheckoutEvent(CheckoutDomainEvent message) => 
        new CheckoutIntegrationEvent
        {
            UserId = message.Source.UserId,
            Total = mesage.Source.Total,
            ...
        };
}
```
