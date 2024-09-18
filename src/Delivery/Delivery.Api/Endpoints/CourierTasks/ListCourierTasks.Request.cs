using Microsoft.AspNetCore.Mvc;
using SharedKernel.Contracts;

namespace Delivery.Api.Endpoints.CourierTasks;

public class ListCourierTasksRequest : ListQueryRequest
{
    [FromRoute] public int? OrganizationId { get; set; }
}