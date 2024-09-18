using System;

namespace Delivery.Domain.Services.Contracts;

public class ListCourierAgentsResponse
{
    public string FullName { get; set; }
    public int ExternalCourierAgentId { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public bool HasGpsAccuracy { get; set; }
    // public TransportType TransportType { get; set; }
    public string TransportDescription { get; set; }
    public string LicensePlate { get; set; }
    public string Tags { get; set; }
    public string Email { get; set; }
    public int CellphoneBatteryLevel { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string PictureUri { get; set; }
}
