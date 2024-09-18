using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using SharedKernel.Contracts;
using Delivery.Domain.Services.Contracts;
using Delivery.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Delivery.Api.Endpoints.CourierAgents;

public class ListCourierAgents : EndpointBaseAsync
    .WithRequest<ListCourierAgentsRequest>
    .WithActionResult<PagedResponse<ListCourierAgentsResponse>>
{
    private readonly ICourierCommunicationService _courierService;

    public ListCourierAgents(ICourierCommunicationService courierService
    )
    {
        _courierService = courierService;
    }

    [HttpGet("/courier-agents")]
    [Authorize(Policy = "UserPolicy")]
    [SwaggerOperation(
        Summary = "List CourierAgents",
        Description = "List CourierAgents",
        OperationId = "CourierAgents.List",
        Tags = new[] { "CourierAgentsEndpoint" })
    ]
    public override async Task<ActionResult<PagedResponse<ListCourierAgentsResponse>>> HandleAsync(
        [FromQuery] ListCourierAgentsRequest request, CancellationToken cancellationToken = default)
    {
        // INFO se que está acoplado, por el momento no necesitamos desacoplarlo porque no agregaremos agentes desde acá
        var courierAgents = await _courierService.GetCourierAgentsAsync();

        if (courierAgents is null)
        {
            return BadRequest("No se logró obtener los Courier Agents");
        }

        var result = new PagedResponse<ListCourierAgentsResponse>(
            courierAgents,
            0,
            courierAgents.Count(),
            courierAgents.Count()
        );
        return Ok(result);
    }
}