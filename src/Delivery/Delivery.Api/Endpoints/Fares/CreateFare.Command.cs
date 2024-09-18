using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Delivery.Api.Endpoints.Fares;


public class CreateFareCommand
{
    [FromRoute] [JsonIgnore] public int OrganizationId { get; set; }
    [FromBody] public CreateFareCommandBody CreateFareCommandBody { get; set; }
}

public class CreateFareCommandBody
{
    [Required] public string OriginLatitude { get; set; }
    [Required] public string OriginLongitude { get; set; }
    [Required] public string DestinationLatitude { get; set; }
    [Required] public string DestinationLongitude { get; set; }
}