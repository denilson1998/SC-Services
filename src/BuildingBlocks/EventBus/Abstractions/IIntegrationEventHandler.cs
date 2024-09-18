namespace Sitec.Delivery.EventBus.Abstractions;
public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
{
    Task Handle(TIntegrationEvent @event);
}

public interface IIntegrationEventHandler
{
}
