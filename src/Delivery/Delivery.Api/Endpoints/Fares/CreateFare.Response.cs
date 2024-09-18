namespace Delivery.Api.Endpoints.Fares;

public class CreateFareResponse
{
    public int Id { get; set; }
    public int EstimatedDistance { get; set; }
    public int EstimatedDuration { get; set; }
    public decimal Price { get; set; }
    public string OriginLatitude { get; set; }
    public string OriginLongitude { get; set; }
    public string DestinationLatitude { get; set; }
    public string DestinationLongitude { get; set; }
}