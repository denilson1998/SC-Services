using Delivery.Domain.Entities;

namespace Delivery.Domain.Services.Contracts;

public class CreateExternalPickupAndDeliveryCommand
{
    public CreateExternalPickupAndDeliveryAddressCommandBody OriginAddress { get; set; }
    public CreateExternalPickupAndDeliveryAddressCommandBody DeliveryAddress { get; set; }
}
public class CreateExternalPickupAndDeliveryAddressCommandBody
{
    public string ClientName { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string PhoneNumber { get; set; }
    public string Reference { get; set; }
}

public class CreateExternalPickupAndDeliveryResponse
{
    public int ExternalTaskId { get; set; }
    public string PickupTrackingLink { get; set; }
    public string DeliveryTrackingLink { get; set; }
    // public Address OriginAddress { get; set; }
    // public Address DeliveryAddress { get; set; }
}
