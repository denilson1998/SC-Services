// using SharedKernel.AbstractEntities;


// namespace Delivery.Domain.Entities;

// public class Route : AuditableEntity
// {
//     public Address Origin { get; protected set; }
//     public Address Destination { get; protected set; }
//     public int EstimatedDistance { get; protected set; }
//     public int EstimatedDuration { get; protected set; }
//     public decimal Price { get; protected set; }
//     public int Order { get; protected set; }
//     public string PartnerTaskId { get; protected set; }

//     public Route(){}
//     public Route(
//         Address origin,
//         Address destination,
//         int estimatedDistance,
//         int estimatedDuration,
//         decimal price,
//         int order
//     )
//     {
//         Origin = origin;
//         Destination = destination;
//         EstimatedDistance = estimatedDistance;
//         EstimatedDuration = estimatedDuration;
//         Price = price;
//         Order = order;
//     }

//     public void Promote(
//         string partnerTaskId
//     )
//     {
//         PartnerTaskId = partnerTaskId;
//     }
// }