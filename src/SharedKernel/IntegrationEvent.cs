namespace SharedKernel;
public record IntegrationEvent
{
    public Guid IntegrationEventId { get; }

    public DateTime IntegrationEventCreationDate { get; }

    public IntegrationEvent()
    {
        IntegrationEventId = Guid.NewGuid();
        IntegrationEventCreationDate = DateTime.UtcNow;
    }
}
