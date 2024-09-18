using SharedKernel;

namespace Sitec.Delivery.EventBus.Abstractions;
public interface IEventBus
{
    Task PublishAsync(IntegrationEvent integrationEvent);
    Task SimplePublishAsync(string topicName);
}
