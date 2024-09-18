// using System;
// using SharedKernel.AbstractEntities;
// using SharedKernel.Interfaces;

// namespace Delivery.Domain.Entities;

// public class CourierAgent : AuditableEntity, ISoftDelete
// {
//     public string FullName { get; }
//     public int ExternalCourierAgentId { get; }
//     public string PhoneNumber { get; }
//     public bool IsActive { get; set; }
//     public bool HasGpsAccuracy { get; set; }
//     public TransportType TransportType { get; set; }
//     public string TransportDescription { get; set; }
//     public string LicensePlate { get; set; }
//     public string Tags { get; set; }
//     public string Email { get; set; }
//     public int CellphoneBatteryLevel { get; set; }
//     public string Latitude { get; set; }
//     public string Longitude { get; set; }
//     public string PictureUri { get; set; }
//     public bool IsAvailable { get; set; }
//     public bool IsDeleted { get; set; }
//     public DateTime? DeletionDateTime { get; set; }

//     public Courier(
//         string firstName,
//         string phoneNumber
//     )
//     {
//         FirstName = firstName;
//         PhoneNumber = phoneNumber;
//     }
// }

// public enum TransportType
// {
//     Car,
//     Motorcycle
// }