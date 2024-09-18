using Dapr.Client;
using Microsoft.Extensions.Logging;
using Sitec.Delivery.EventBus.Abstractions;
using SharedKernel;
namespace Sitec.Delivery.EventBus;
public class DaprEventBus : IEventBus
{
    private readonly DaprClient _dapr;
    private readonly ILogger _logger;

    public DaprEventBus(DaprClient dapr, ILogger<DaprEventBus> logger)
    {
        _dapr = dapr;
        _logger = logger;
    }

    public async Task PublishAsync(IntegrationEvent integrationEvent)
    {
        var topicName = integrationEvent.GetType().Name;

        _logger.LogInformation(
            "Publishing event {@Event} to {PubsubName}.{TopicName}",
            integrationEvent,
            GlobalConstanst.DAPR_PUBSUB_NAME,
            topicName);

        // We need to make sure that we pass the concrete type to PublishEventAsync,
        // which can be accomplished by casting the event to dynamic. This ensures
        // that all event fields are properly serialized.
        await _dapr.PublishEventAsync(GlobalConstanst.DAPR_PUBSUB_NAME, topicName, (object)integrationEvent);
    }

    public async Task SimplePublishAsync(string topicName)
    {
        _logger.LogInformation(
            "Simple Publishing event SimpleEvent to {PubsubName}.{TopicName}",
            GlobalConstanst.DAPR_PUBSUB_NAME,
            topicName);

        // We need to make sure that we pass the concrete type to PublishEventAsync,
        // which can be accomplished by casting the event to dynamic. This ensures
        // that all event fields are properly serialized.
        await _dapr.PublishEventAsync(GlobalConstanst.DAPR_PUBSUB_NAME, topicName);
    }
}
