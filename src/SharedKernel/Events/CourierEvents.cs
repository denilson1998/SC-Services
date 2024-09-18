namespace SharedKernel.Events;
public record CourierTaskUpdatedEventList : IntegrationEvent
{
    public int OrganizationId { get; set; }
    public CourierTaskUpdatedEvent Body { get; set; }
}
public record CourierTaskUpdatedEvent
{
    public int CourierTaskId { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? SucceededAt { get; set; }
    public DateTime? CanceledAt { get; set; }
}
