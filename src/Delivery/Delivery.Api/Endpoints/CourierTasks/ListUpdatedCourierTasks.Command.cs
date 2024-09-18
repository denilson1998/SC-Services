using System;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Contracts;

namespace Delivery.Api.Endpoints.CourierTasks;

public class ListUpdatedCourierTasksCommand : ListQueryRequest
{
    [FromRoute] public int OrganizationId { get; set; }
    [FromQuery] public DateTime LastUpdated { get; set; }
}