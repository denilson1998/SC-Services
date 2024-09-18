using System;
using SharedKernel.AbstractEntities;

namespace Delivery.Domain.Entities;

public class Fare : AuditableEntity
{
    public int OrganizationId { get; protected set; }
    public int EstimatedDistance { get; protected set; }
    public int EstimatedDuration { get; protected set; }
    public decimal Price { get; protected set; }
    public string OriginLatitude { get; protected set; }
    public string OriginLongitude { get; protected set; }
    public string DestinationLatitude { get; protected set; }
    public string DestinationLongitude { get; protected set; }
    public Fare(){}
    public Fare(
        int organizationId,
        int estimatedDistance,
        int estimatedDuration,
        decimal price,
        string originLatitude,
        string originLongitude,
        string destinationLatitude,
        string destinationLongitude
    )
    {
        OrganizationId = organizationId;
        EstimatedDistance = estimatedDistance;
        EstimatedDuration = estimatedDuration;
        Price = price;
        OriginLatitude = originLatitude;
        OriginLongitude = originLongitude;
        DestinationLatitude = destinationLatitude;
        DestinationLongitude = destinationLongitude;
    }
}
