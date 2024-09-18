using SharedKernel.AbstractEntities;
using SharedKernel.Interfaces;
using System;

namespace Delivery.Domain.Entities;

public class CourierTask : AuditableEntity, IMultiTenant
{
    public string ExternalTaskId { get; set; }
    public int OrganizationId { get; set; }
    public int BillId { get; protected set; }
    public Fare Fare { get; set; }
    public DateTime? PaidAt { get; protected set; }
    public DateTime? PickedUpAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string PickupTrackingLink { get; protected set; }
    public string DeliveryTrackingLink { get; protected set; }

    #region Origin

    public string OriginLatitude { get; protected set; }
    public string OriginLongitude { get; protected set; }
    public string OriginClientName { get; protected set; }
    public string OriginPhoneNumber { get; protected set; }
    public string OriginAddressReference { get; protected set; }

    #endregion Origin

    #region Destination

    public string DestinationLatitude { get; protected set; }
    public string DestinationLongitude { get; protected set; }
    public string DestinationClientName { get; protected set; }
    public string DestinationPhoneNumber { get; protected set; }
    public string DestinationAddressReference { get; protected set; }

    #endregion Destination

    #region States

    public DateTime? AssignedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? ArrivedAt { get; set; } // no se usaba
    public DateTime? CompletedAt { get; set; } // no se usaba
    public DateTime? SucceededAt { get; set; }
    public DateTime? FailedAt { get; set; }
    public DateTime? CanceledAt { get; set; }

    public DateTime? LastStateChangeAt { get; set; }

    #endregion States

    public CourierTask()
    {
    }

    public CourierTask(
        Fare fare,
        int billId,
        int organizationId
    )
    {
        Fare = fare;
        OriginLatitude = fare.OriginLatitude;
        OriginLongitude = fare.OriginLongitude;
        DestinationLatitude = fare.DestinationLatitude;
        DestinationLongitude = fare.DestinationLongitude;
        BillId = billId;
        OrganizationId = organizationId;
    }

    public void SetOriginAddress(
        string clientName,
        string phoneNumber,
        string addressReference
    )
    {
        OriginClientName = clientName;
        OriginPhoneNumber = phoneNumber;
        OriginAddressReference = addressReference;
    }

    public void SetDestinationAddress(
        string clientName,
        string phoneNumber,
        string addressReference
    )
    {
        DestinationClientName = clientName;
        DestinationPhoneNumber = phoneNumber;
        DestinationAddressReference = addressReference;
    }

    public void MarkAsPaid()
    {
        PaidAt = DateTime.Now; // TODO cambiar a hora de fassil
    }

    public void SetExternalTaskInformation(
        string externalTaskId,
        string pickupTrackingLink,
        string deliveryTrackingLink
    )
    {
        ExternalTaskId = externalTaskId;
        PickupTrackingLink = pickupTrackingLink;
        DeliveryTrackingLink = deliveryTrackingLink;
    }
}