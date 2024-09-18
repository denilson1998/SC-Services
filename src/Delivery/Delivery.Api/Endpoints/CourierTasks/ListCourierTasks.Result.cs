using System;

namespace Delivery.Api.Endpoints.CourierTasks;
public class ListCourierTasksResult
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

    // Origin
    public string OriginLatitude { get; set; }
    public string OriginLongitude { get; set; }
    public string OriginClientName { get; set; }
    public string OriginPhoneNumber { get; set; }
    public string OriginAddressReference { get; set; }

    // Destination
    public string DestinationLatitude { get; set; }
    public string DestinationLongitude { get; set; }
    public string DestinationClientName { get; set; }
    public string DestinationPhoneNumber { get; set; }
    public string DestinationAddressReference { get; set; }

    // States
    public DateTime? AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ArrivedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? SucceededAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
}
