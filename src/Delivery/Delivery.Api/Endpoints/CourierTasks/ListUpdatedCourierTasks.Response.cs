using System;

namespace Delivery.Api.Endpoints.CourierTasks;

public class ListUpdatedCourierTasksResponse
{
    public int Id { get; set; }
    public string ExternalTaskId { get; set; }
    public int OrganizationId { get; set; }
    public int BillId { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string PickupTrackingLink { get; set; }
    public string DeliveryTrackingLink { get; set; }
    public DateTime? AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? SucceededAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
}