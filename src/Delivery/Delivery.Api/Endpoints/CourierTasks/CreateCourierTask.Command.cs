using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Delivery.Api.Endpoints.CourierTasks;

// orderId
public class CreateCourierTaskCommand
{
    [Required] [FromRoute] [JsonIgnore] public int OrganizationId { get; set; }
    [Required] [FromBody] public CreateCourierTaskCommandBody CreateCourierTaskCommandBody { get; set; }
}

public class CreateCourierTaskCommandBody
{
    public int FareId { get; set; }
    public CreateCourierTaskAddressCommand Origin { get; set; }
    public CreateCourierTaskAddressCommand Destination { get; set; }
}

public class CreateCourierTaskAddressCommand
{
    [Required] public string PhoneNumber { get; set; }
    [Required] public string ClientName { get; set; }
    [Required] public string AddressReference { get; set; }
}